using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PayPeriod;
using WeHire.Application.DTOs.PaySlip;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.ExcelServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.PayPeriodEnum;

namespace WeHire.Infrastructure.Services.PayPeriodServices
{
    public class PayPeriodService : IPayPeriodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExcelService _excelService;

        public PayPeriodService(IUnitOfWork unitOfWork, IMapper mapper, IExcelService excelService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _excelService = excelService;
        }

        public async Task<GetPayPeriodBill> GetPayPeriodByProject(int projectId, DateTime inputDate)
        {
            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId)
                                                             .Include(p => p.Company)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_EXIST);

            var durationInMonth = GetPayPeriodDuration(project.StartDate, project.EndDate, inputDate);

            var payPeriod = await _unitOfWork.PayPeriodRepository.Get(p => p.ProjectId == projectId &&
                                                                           p.StartDate == durationInMonth.StartDate &&
                                                                           p.EndDate == durationInMonth.EndDate)
                                                                 .Include(p => p.PaySlips)
                                                                 .ThenInclude(ps => ps.HiredDeveloper)
                                                                 .ThenInclude(h => h.Developer)
                                                                 .ThenInclude(d => d.User)
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "PayPeriod", "PayPeriod does not exist!!");

            var totalOTAmount = 0M;
            var totalActualAmount = 0M;
            var developerFullNames = new List<string>();
            foreach(var paySlip in payPeriod.PaySlips)
            {
                totalOTAmount += GetTotalOTAmountByDeveloperCode(paySlip.HiredDeveloperId, 0, paySlip.TotalOvertimeHours).TotalOtAmount;
                totalActualAmount += GetTotalOTAmountByDeveloperCode(paySlip.HiredDeveloperId, paySlip.TotalActualWorkedHours, 0).TotalActualAmount;
                developerFullNames.Add($"{paySlip.HiredDeveloper.Developer.User.FirstName} {paySlip.HiredDeveloper.Developer.User.LastName}");
            }

            var mappedPayPeriod = _mapper.Map<GetPayPeriodBill>(payPeriod);
            mappedPayPeriod.TotalActualAmount = totalActualAmount.ToString("#,##0 VND"); ;
            mappedPayPeriod.TotalOTAmount = totalOTAmount.ToString("#,##0 VND"); ;
            mappedPayPeriod.TotalAmount = payPeriod.TotalAmount?.ToString("#,##0 VND");
            mappedPayPeriod.DeveloperFullName = developerFullNames;
            return mappedPayPeriod;
        }

        public async Task<GetPayPeriodModel> CreatePayPeriod(CreatePayPeriodModel requestBody)
        {
            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == requestBody.ProjectId)
                                                             .Include(p => p.PayPeriods)
                                                             .Include(p => p.HiredDevelopers)
                                                             .ThenInclude(h => h.Contract)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_EXIST);

            var payPeriodDuration = GetPayPeriodDuration(project.StartDate, project.EndDate, requestBody.InputDate);

            var isExistPayPeriod = await _unitOfWork.PayPeriodRepository.AnyAsync(p => p.ProjectId == requestBody.ProjectId &&
                                                                                       p.StartDate == payPeriodDuration.StartDate &&
                                                                                       p.EndDate == payPeriodDuration.EndDate);
            if (isExistPayPeriod)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "PayPeriod", "PayPeriod is exist for this month, please update if you want to change!");

            var anyDevWorked = project.HiredDevelopers.Where(h => (h.Contract.FromDate >= payPeriodDuration.StartDate ||
                                                                   h.Contract.ToDate <= payPeriodDuration.EndDate) &&
                                                                   h.Contract.Status == (int)ContractEnum.ContractStatus.Signed)
                                                       .ToList();
            if (!anyDevWorked.Any())
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Project", $"There are no developers worked in {payPeriodDuration.Month}!!");
            }

            var newPayPeriod = new PayPeriod
            {
                ProjectId = requestBody.ProjectId,
                PayPeriodCode = await GenerateUniqueCodeName(),
                StartDate = payPeriodDuration.StartDate,
                EndDate = payPeriodDuration.EndDate,
                TotalAmount = 0,
                CreatedAt = DateTime.Now,
                Status = (int)PayPeriodStatus.Created,
                PaySlips = project.HiredDevelopers
                .Where(p => p.Contract.FromDate < payPeriodDuration.EndDate && p.Contract.ToDate > payPeriodDuration.StartDate)
                .Select(p =>
                {
                    var dayRangeInMonth = ConvertDateTime.ConvertStringListToDateList(payPeriodDuration.DayRangeInMonth);

                    var filteredWorkDays = dayRangeInMonth
                        .Where(d => d >= p.Contract.FromDate && d <= p.Contract.ToDate)
                        .ToList();

                    return new PaySlip
                    {
                        HiredDeveloperId = p.HiredDeveloperId,
                        TotalActualWorkedHours = 0,
                        TotalOvertimeHours = 0,
                        TotalEarnings = 0,
                        CreatedAt = DateTime.Now,

                        WorkLogs = filteredWorkDays.Select(d => new WorkLog
                        {
                            WorkDate = d,
                            TimeIn = null,
                            TimeOut = null
                        }).ToList()
                    };
                }).ToList()
            };
            await _unitOfWork.PayPeriodRepository.InsertAsync(newPayPeriod);
            await _unitOfWork.SaveChangesAsync();

            var mappedPayPeriod = _mapper.Map<GetPayPeriodModel>(newPayPeriod);
            return mappedPayPeriod;
        }


        public async Task InsertPayPeriodFromExcel(int projectId, ImportPaySlipModel importPaySlipModel)
        {
            var isExistPayPeriod = await _unitOfWork.PayPeriodRepository.AnyAsync(p => p.ProjectId == projectId &&
                                                                                       p.StartDate == importPaySlipModel.StartDate &&
                                                                                       p.EndDate == importPaySlipModel.EndDate);
            if (isExistPayPeriod)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "PayPeriod", "PayPeriod is exist for this month, please update if you want to change!");

            var developers = _unitOfWork.HiredDeveloperRepository
                              .Get(h => h.ProjectId == projectId)
                              .Include(h => h.Developer)
                              .Select(h => h.Developer)
                              .ToList();
            var hiredDeveloper = _unitOfWork.HiredDeveloperRepository.Get(h => h.ProjectId == projectId && h.Contract.Status == (int)ContractEnum.ContractStatus.Signed)
                                                                     .Include(h => h.Contract)
                                                                     .ToList();

            var developerCodes = importPaySlipModel.PaySlips.Select(p => p.CodeName).ToList();

            HandleDeveloperInProject(developers, developerCodes!);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                var newPayPeriod = new PayPeriod
                {
                    ProjectId = projectId,
                    PayPeriodCode = await GenerateUniqueCodeName(),
                    StartDate = importPaySlipModel.StartDate,
                    EndDate = importPaySlipModel.EndDate,
                    CreatedAt = DateTime.Now,
                    Status = (int)PayPeriodStatus.Created,
                    PaySlips = importPaySlipModel.PaySlips.Select(p => new PaySlip
                    {
                        HiredDeveloperId = GetHiredDeveloperId(p.CodeName),
                        TotalOvertimeHours = p.TotalOvertimeHours,
                        CreatedAt = DateTime.Now,
                        WorkLogs = p.WorkLogs.Select(w => new WorkLog
                        {
                            WorkDate = ConvertDateTime.ConvertStringToDateTimeExcel(w.WorkDate),
                            TimeIn = w.TimeIn,
                            TimeOut = w.TimeOut,
                        }).ToList()
                    }).ToList()
                };

                foreach (var paySlip in newPayPeriod.PaySlips)
                {
                    decimal totalWorkedHours = 0;

                    foreach (var workLog in paySlip.WorkLogs)
                    {
                        if (workLog.TimeIn != null && workLog.TimeOut != null)
                        {
                            if (workLog.TimeIn < new TimeSpan(8, 0, 0))
                            {
                                workLog.TimeIn = new TimeSpan(8, 0, 0);
                            }

                            TimeSpan workedHours = workLog.TimeOut.Value - workLog.TimeIn.Value;

                            if (workedHours.TotalHours > 0)
                            {
                                int lunchBreakMinutes = 0;
                                if (workLog.TimeOut >= new TimeSpan(13, 0, 0))
                                {
                                    lunchBreakMinutes = 60;
                                }
                                totalWorkedHours = (decimal)(workedHours.TotalHours - (lunchBreakMinutes / 60.0));
                                totalWorkedHours += (decimal)workedHours.TotalHours;
                            }
                            else
                            {
                                throw new ExceptionResponse(HttpStatusCode.BadRequest, "WorkLog", "Time in or Time out is invalid");
                            }
                        }
                    }
                    paySlip.TotalActualWorkedHours = totalWorkedHours;
                    paySlip.TotalEarnings = GetTotalEarningByDeveloperCode(paySlip.HiredDeveloperId, paySlip.TotalActualWorkedHours, paySlip.TotalOvertimeHours) ?? 0;
                }

                newPayPeriod.TotalAmount = newPayPeriod.PaySlips.Sum(p => p.TotalEarnings ?? 0);
                await _unitOfWork.PayPeriodRepository.InsertAsync(newPayPeriod);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<ExcelFileModel> GeneratePayPeriodExcelFile(int projectId, DateTime inputDate)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);

            var durationInMonth = GetPayPeriodDuration(project.StartDate, project.EndDate, inputDate);

            var hiredDev = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ProjectId == projectId && h.Contract.Status == (int)ContractEnum.ContractStatus.Signed &&
                                                                         (h.Contract.FromDate <= durationInMonth.StartDate ||
                                                                          h.Contract.ToDate >= durationInMonth.EndDate))
                                                                     .Include(h => h.Developer)
                                                                        .ThenInclude(d => d.User)
                                                                     .Include(d => d.Contract)
                                                                     .Include(d => d.JobPosition)
                                                                     .ToListAsync();
            if (!hiredDev.Any())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Project", $"There are no developers worked in {durationInMonth.Month}!!");

            var company = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId)
                                                             .Select(p => p.Company)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var fileName = $"EmployeePaySlip_{company.CompanyName}_{inputDate.ToString("MMMM")}_{inputDate.ToString("yyyy")}";

            var paySlips = hiredDev.Select(h => new PaySlipModel
            {
                Fullname = $"{h.Developer.User.FirstName} {h.Developer.User.LastName}",
                CodeName = h.Developer.CodeName,
                Position = h.JobPosition.PositionName,
                BasicSalary = h.Contract.BasicSalary,
                FromDate = h.Contract.FromDate,
                ToDate = h.Contract.ToDate,
            }).ToList();

            var excelByteArray = _excelService.ExportExcelFile(company.CompanyName, durationInMonth.StartDateString, durationInMonth.EndDateString, durationInMonth.Month, durationInMonth.DayCountInMonth, durationInMonth.DayRangeInMonth, paySlips);
            return new ExcelFileModel
            {
                FileName = fileName.Replace(" ", ""),
                ExcelByteArray = excelByteArray
            };
        }

        public async Task<List<PayPeriodInMonth>> GetPayPeriodsInProjectDurationAsync(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            var payPeriods = new List<PayPeriodInMonth>();

            if (project != null)
            {
                DateTime startDate = (DateTime)project.StartDate!;
                DateTime endDate = (DateTime)project.EndDate!;

                var firstMonthPayPeriod = new PayPeriodInMonth
                {
                    MonthYearMMMM = startDate.ToString("MMMM yyyy"),
                    InputDate = "25/" + startDate.ToString("MM/yyyy"),
                    StartDateMMM = ConvertDateTime.ConvertDateToString(startDate),
                    StartDate = startDate.ToString("dd/MM/yyyy"),
                    EndDateMMM = ConvertDateTime.ConvertDateToString(new DateTime(startDate.Year, startDate.Month, 24)),
                    EndDate = new DateTime(startDate.Year, startDate.Month, 24).ToString("dd/MM/yyyy")
                };

                payPeriods.Add(firstMonthPayPeriod);

                while (startDate < endDate)
                {
                    startDate = new DateTime(startDate.Year, startDate.Month, 25).AddMonths(1);

                    var payPeriod = new PayPeriodInMonth();

                    if (startDate > endDate)
                    {
                        payPeriod = new PayPeriodInMonth
                        {
                            MonthYearMMMM = startDate.ToString("MMMM yyyy"),
                            InputDate = "25/" + startDate.ToString("MM/yyyy"),
                            StartDateMMM = ConvertDateTime.ConvertDateToString(startDate.AddMonths(-1)),
                            StartDate = ConvertDateTime.ConvertDateToStringNumberThreeline(startDate.AddMonths(-1)),
                            EndDateMMM = ConvertDateTime.ConvertDateToString(endDate),
                            EndDate = endDate.ToString("dd/MM/yyyy")
                        };
                    }
                    else
                    {
                        payPeriod = new PayPeriodInMonth
                        {
                            MonthYearMMMM = startDate.ToString("MMMM yyyy"),
                            InputDate = "25/" + startDate.ToString("MM/yyyy"),
                            StartDateMMM = ConvertDateTime.ConvertDateToString(startDate.AddMonths(-1)),
                            StartDate = ConvertDateTime.ConvertDateToStringNumberThreeline(startDate.AddMonths(-1)),

                            EndDateMMM = ConvertDateTime.ConvertDateToString(new DateTime(startDate.Year, startDate.Month, 24)),
                            EndDate = new DateTime(startDate.Year, startDate.Month, 24).ToString("dd/MM/yyyy")
                        };
                    }

                    payPeriods.Add(payPeriod);
                }
            }

            return payPeriods;
        }


        public DurationInMonth GetPayPeriodDuration(DateTime? startProjectDate, DateTime? endProjectDate, DateTime dateInput)
        {
            int year = dateInput.Year;
            int month = dateInput.Month;

            int limitStart = 25;
            int limitEnd = 24;
            //int daysInMonth = DateTime.DaysInMonth(year, month);

            DateTime firstDay = new DateTime(year, month, limitStart).AddMonths(-1);
            DateTime lastDay = new DateTime(year, month, limitEnd);

            //if (dateInput < startProjectDate)
            //    throw new ExceptionResponse(HttpStatusCode.BadRequest, "DateInput", "Your entry date must be within the start and end period of the project!!");

            //if (firstDay > endProjectDate)
            //    throw new ExceptionResponse(HttpStatusCode.BadRequest, "DateInput", "There are no pay period for this month!!");

            if (dateInput >= new DateTime(year, month, 1) && dateInput <= new DateTime(year, month, 10))
            {
                firstDay = new DateTime(year, month, limitStart).AddMonths(-2);
                //firstDay = (startProjectDate > firstDay) ? startProjectDate.Value : firstDay;

                lastDay = new DateTime(year, month, limitEnd).AddMonths(-1);
                //lastDay = (endProjectDate < lastDay) ? endProjectDate.Value : lastDay;
            }
            //else if (startProjectDate > firstDay)
            //{
            //    firstDay = startProjectDate.Value;
            //}
            //else if (endProjectDate < lastDay)
            //{
            //    lastDay = endProjectDate.Value;
            //}

            var dayCountInMonth = 0;
            List<string> dayRange = new List<string>();

            for (DateTime date = firstDay; date <= lastDay; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    dayCountInMonth++;
                    dayRange.Add(date.ToString("dd/MM/yyyy"));
                }
            }

            return new DurationInMonth
            {
                Month = lastDay.ToString("MMMM yyyy"),
                StartDate = firstDay,
                EndDate = lastDay,
                StartDateString = ConvertDateTime.ConvertDateToStringNumberThreeline(firstDay),
                EndDateString = ConvertDateTime.ConvertDateToStringNumberThreeline(lastDay),
                DayCountInMonth = dayCountInMonth,
                DayRangeInMonth = dayRange
            };
        }
        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistRequestCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "PAY" + randomNumber.ToString();
                isExistRequestCode = await _unitOfWork.PayPeriodRepository.AnyAsync(d => d.PayPeriodCode == codeName);
            } while (isExistRequestCode);
            return codeName;
        }

        public void HandleDeveloperInProject(List<Developer> developers, List<string> developerCodes)
        {
            var developerCodesSet = new HashSet<string>(developers.Select(dev => dev.CodeName));

            var nonExistentDeveloperCodes = developerCodes.Where(code => !developerCodesSet.Contains(code)).ToList();

            if (nonExistentDeveloperCodes.Any())
            {
                var nonExistentCodes = string.Join(", ", nonExistentDeveloperCodes);
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Developer", $"Developer with codes '{nonExistentCodes}' do not exist in the project.");
            }

            foreach (var developer in developers)
            {
                if (!developerCodesSet.Contains(developer.CodeName))
                {
                    throw new ExceptionResponse(HttpStatusCode.BadRequest, "Developer", $"Developer with code '{developer.CodeName}' does not exist or missed in the provided Excel.");
                }
            }
        }

        private int GetHiredDeveloperId(string codeName)
        {
            var hiredDeveloperId = _unitOfWork.HiredDeveloperRepository.Get(h => h.Developer.CodeName == codeName)
                                                                       .Include(h => h.Developer)
                                                                       .Select(h => h.HiredDeveloperId)
                                                                       .SingleOrDefault();
            return hiredDeveloperId;
        }

        public decimal? GetTotalEarningByDeveloperCode(int? hiredDeveloperId, decimal? totalActualWorkedHours, decimal? overtimeHours)
        {
            var contract = _unitOfWork.HiredDeveloperRepository.Get(h => h.HiredDeveloperId == hiredDeveloperId)
                                                               .Include(h => h.Developer)
                                                               .Include(h => h.Contract)
                                                               .Select(h => h.Contract)
                                                               .SingleOrDefault();
            var totalEarning = TotalEarningCalculation(contract.StandardMonthlyWorkingHours,
                                                       totalActualWorkedHours,
                                                       overtimeHours,
                                                       contract.OvertimePayMultiplier,
                                                       contract.BasicSalary);
            return Math.Round(totalEarning, 2);
        }

        public decimal TotalEarningCalculation(int? standardMonthlyWorkingHours,
                                                decimal? totalActualWorkedHours,
                                                decimal? overtimeHours,
                                                decimal? overtimeMultiplier,
                                                decimal? basicSalary)
        {
            decimal salaryPerHour = (decimal)(basicSalary / standardMonthlyWorkingHours);
            var totalEarning = (salaryPerHour * totalActualWorkedHours) + (salaryPerHour * overtimeMultiplier * overtimeHours);
            return (decimal)totalEarning;
        }

        public TotalAmountModel GetTotalOTAmountByDeveloperCode(int? hiredDeveloperId, decimal? totalActualWorkedHours, decimal? overtimeHours)
        {
            var contract = _unitOfWork.HiredDeveloperRepository.Get(h => h.HiredDeveloperId == hiredDeveloperId)
                                                               .Include(h => h.Developer)
                                                               .Include(h => h.Contract)
                                                               .Select(h => h.Contract)
                                                               .SingleOrDefault();
            var totalOT = TotalOTMoneyAmount(contract.StandardMonthlyWorkingHours,
                                                 overtimeHours,
                                                 contract.OvertimePayMultiplier,
                                                 contract.BasicSalary);
            var totalActual = TotalMoneyActualAmount(contract.StandardMonthlyWorkingHours,
                                                 totalActualWorkedHours,
                                                 contract.BasicSalary);
            return new TotalAmountModel
            {
                TotalOtAmount = totalOT,
                TotalActualAmount = totalActual,
            };
        }

        public decimal TotalMoneyActualAmount(int? standardMonthlyWorkingHours,
                                                decimal? totalActualWorkedHours,
                                                decimal? basicSalary)
        {
            decimal salaryPerHour = (decimal)(basicSalary / standardMonthlyWorkingHours);
            var total = (salaryPerHour * totalActualWorkedHours);
            return (decimal)total;
        }

        public decimal TotalOTMoneyAmount(int? standardMonthlyWorkingHours,
                                         decimal? overtimeHours,
                                         decimal? overtimeMultiplier,
                                         decimal? basicSalary)
        {
            decimal salaryPerHour = (decimal)(basicSalary / standardMonthlyWorkingHours);
            var totalOTMoneyAmount = (salaryPerHour * overtimeMultiplier * overtimeHours);
            return (decimal)totalOTMoneyAmount;
        }
    }
}
