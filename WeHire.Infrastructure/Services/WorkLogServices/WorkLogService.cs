using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.WorkLog;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.PayPeriodServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.PayPeriodEnum;

namespace WeHire.Infrastructure.Services.WorkLogServices
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayPeriodService _payPeriodService;
        private readonly IMapper _mapper;

        public WorkLogService(IUnitOfWork unitOfWork, IMapper mapper, IPayPeriodService payPeriodService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payPeriodService = payPeriodService;
        }

        public async Task<List<GetWorkLogModel>> GetWorkLogByPaySlipIdAsync(int paySlipId)
        {
            var workLogs = await _unitOfWork.WorkLogRepository.Get(w => w.PaySlipId == paySlipId)
                                                              .OrderBy(x => x.WorkDate)
                                                              .ToListAsync();
            var mappedWorkLog = _mapper.Map<List<GetWorkLogModel>>(workLogs);
            return mappedWorkLog;
        }

        public async Task<WorkLogResponseModel> UpdateWorkLogAsync(UpdateWorkLogModel requestBody)
        {
            var workLog = await _unitOfWork.WorkLogRepository.GetByIdAsync(requestBody.WorkLogId)
                    ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "workLog", "WorkLog does not exist!!");

            var oldWorkLog = new WorkLog();
            oldWorkLog.TimeIn = workLog.TimeIn;
            oldWorkLog.TimeOut = workLog.TimeOut;
            oldWorkLog.IsPaidLeave = workLog.IsPaidLeave;

            var updatedWorkLog = _mapper.Map(requestBody, workLog);

            var paySlip = await _unitOfWork.PaySlipRepository.GetByIdAsync(updatedWorkLog.PaySlipId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "paySlip", "paySlip does not exist!!");

            var payPeriod = await _unitOfWork.PayPeriodRepository.Get(p => p.PayPeriodId == paySlip.PayPeriodId &&
                                                                           p.Status == (int)PayPeriodStatus.Created)
                                                                 .Include(p => p.PaySlips)
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "payPeriod", "payPeriod does not exist!!");

            var hourWorkInDayOld = ConvertTime.CalculateTotalWorkTime(oldWorkLog.TimeIn, oldWorkLog.TimeOut);
            var hourWorkInDayNew = 0M;

            if (requestBody.IsPaidLeave == null)
            {
                if(oldWorkLog.IsPaidLeave == true)
                {
                    hourWorkInDayOld = 8M;
                }
                hourWorkInDayNew = ConvertTime.CalculateTotalWorkTime(updatedWorkLog.TimeIn, updatedWorkLog.TimeOut);
            } 
            else if((bool)requestBody.IsPaidLeave)
            {
                hourWorkInDayNew = 8M;
                updatedWorkLog.TimeIn = null;
                updatedWorkLog.TimeOut = null;
            }
            else
            {
                hourWorkInDayNew = 0M;
                updatedWorkLog.TimeIn = null;
                updatedWorkLog.TimeOut = null;
            }

            var totalActualWorkedHours = (paySlip.TotalActualWorkedHours - hourWorkInDayOld) + hourWorkInDayNew;

            paySlip.TotalActualWorkedHours = totalActualWorkedHours;

            paySlip.TotalEarnings = _payPeriodService.GetTotalEarningByDeveloperCode(paySlip.HiredDeveloperId, totalActualWorkedHours, paySlip.TotalOvertimeHours);

            _unitOfWork.WorkLogRepository.Update(updatedWorkLog);

            payPeriod.TotalAmount = payPeriod.PaySlips.Sum(p => p.TotalEarnings ?? 0);
            payPeriod.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            return new WorkLogResponseModel
            {
                PayPeriodId = payPeriod.PayPeriodId,
                PaySlipId = paySlip.PaySlipId,
                WorkLogId = workLog.WorkLogId,
                TotalAmount = payPeriod.TotalAmount?.ToString("#,##0 VND"),
                TotalActualWorkedHours = paySlip.TotalActualWorkedHours,
                TotalEarnings = paySlip.TotalEarnings?.ToString("#,##0 VND"),
                HourWorkInDay = Math.Round(hourWorkInDayNew, 1)
            };
        }
    }
}
