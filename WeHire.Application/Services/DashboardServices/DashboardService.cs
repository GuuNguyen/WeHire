using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Dashboard;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Infrastructure.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.PayPeriodEnum;
using static WeHire.Domain.Enums.ProjectEnum;
using static WeHire.Domain.Enums.TransactionEnum;

namespace WeHire.Application.Services.DashboardServices
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DashboardByAdmin> GetDashboardByAdminAsync()
        {
            var account = _unitOfWork.UserRepository.GetAll();
            var project = _unitOfWork.ProjectRepository.GetAll()
                                                       .Include(p => p.HiringRequests)
                                                       .Include(p => p.PayPeriods)
                                                       .ToList();

            var hiringRequest = _unitOfWork.ReportRepository.Get(h => h.Status != (int)HiringRequestStatus.Saved)
                                                            .AsNoTracking()
                                                            .ToList();

            DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var transaction = _unitOfWork.TransactionRepository.Get(t => t.Status == (int)TransactionStatus.Success).ToList();

            var transactionsInCurrentMonth = transaction
                .Where(t => t.Timestamp >= firstDayOfMonth && t.Timestamp <= lastDayOfMonth)
                .ToList();

            var totalMoney = transaction.Sum(t => t.Amount) * 8/100;

            return new DashboardByAdmin
            {
                AccountDashboard = new AccountDashboard
                {
                    TotalUser = await account.CountAsync(),
                    TotalAdmin = await account.Where(a => a.RoleId == (int)RoleEnum.Admin).CountAsync(),
                    TotalManager = await account.Where(a => a.RoleId == (int)RoleEnum.Manager).CountAsync(),
                    TotalStaff = await account.Where(a => a.RoleId == (int)RoleEnum.Staff).CountAsync(),
                    TotalDeveloper = await account.Where(a => a.RoleId == (int)RoleEnum.Developer).CountAsync(),
                    TotalHumanResource = await account.Where(a => a.RoleId == (int)RoleEnum.HR).CountAsync(),                  
                },
                TotalMoney = totalMoney.ToString("#,##0 VND"),
                TotalProject = project.Count(),
                TotalHiringRequest = hiringRequest.Count(),
            };
        }

        public async Task<DashboardByProject> GetDashboardByProjectAsync(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId)
                                                       .Include(p => p.HiredDevelopers)
                                                       .Include(p => p.HiringRequests)
                                                       .SingleOrDefaultAsync();
            var hiredDevelopers = project.HiredDevelopers.Where(h => h.Status == (int)HiredDeveloperStatus.Working ||
                                                                     h.Status == (int)HiredDeveloperStatus.Terminated ||
                                                                     h.Status == (int)HiredDeveloperStatus.Completed)
                                                         .ToList();
            var hiringRequests = project.HiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Saved ||
                                                                   h.Status == (int)HiringRequestStatus.WaitingApproval ||
                                                                   h.Status == (int)HiringRequestStatus.InProgress ||
                                                                   h.Status == (int)HiringRequestStatus.Rejected ||
                                                                   h.Status == (int)HiringRequestStatus.Completed ||
                                                                   h.Status == (int)HiringRequestStatus.Closed ||
                                                                   h.Status == (int)HiringRequestStatus.Expired)
                                                        .ToList();

            return new DashboardByProject
            {
                DeveloperDashboard = new DeveloperDashboard
                {
                    TotalHiredDeveloper = hiredDevelopers.Count(),
                    TotalWorkingDeveloper = hiredDevelopers.Where(h => h.Status == (int)HiredDeveloperStatus.Working).Count(),
                    TotalTerminatedDeveloper = hiredDevelopers.Where(h => h.Status == (int)HiredDeveloperStatus.Terminated).Count(),
                    TotalCompletedDeveloper = hiredDevelopers.Where(h => h.Status == (int)HiredDeveloperStatus.Completed).Count(),
                },
                HiringRequestDashboard = new HiringRequestDashboard
                {
                    TotalHiringRequest = hiringRequests.Count(),
                    TotalSaved = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Saved).Count(),
                    TotalWaitingApproval = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.WaitingApproval).Count(),
                    TotalInProcess = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.InProgress).Count(),
                    TotalRejected = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Rejected).Count(),
                    TotalCompleted = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Completed).Count(),
                    TotalClosed = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Closed).Count(),
                    TotalExpired = hiringRequests.Where(h => h.Status == (int)HiringRequestStatus.Expired).Count(),
                }
            };
        }

        public Dictionary<DayOfWeek, int> GetProjectDashboard(DateTime dateInWeek)
        {
            Dictionary<DayOfWeek, int> projectsPerDay = new Dictionary<DayOfWeek, int>();

            var projects = _unitOfWork.ProjectRepository.GetAll();

            DayOfWeek startOfWeek = DayOfWeek.Monday; 

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                projectsPerDay[day] = 0;
            }

            DateTime start = dateInWeek;
            while (start.DayOfWeek != startOfWeek)
            {
                start = start.AddDays(-1);
            }

            DateTime end = start.AddDays(7);

            foreach (var project in projects)
            {
                if (project.CreatedAt >= start && project.CreatedAt < end)
                {
                    DayOfWeek dayOfWeek = project.CreatedAt.Value.DayOfWeek;
                    projectsPerDay[dayOfWeek]++;
                }
            }

            return projectsPerDay;
        }

        public Dictionary<DayOfWeek, int> GetHiringRequestDashboard(DateTime dateInWeek)
        {
            Dictionary<DayOfWeek, int> requestPerDay = new Dictionary<DayOfWeek, int>();

            var requests = _unitOfWork.RequestRepository.Get(r => r.Status != (int)HiringRequestStatus.Saved).ToList();

            DayOfWeek startOfWeek = DayOfWeek.Monday;

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                requestPerDay[day] = 0;
            }

            DateTime start = dateInWeek;
            while (start.DayOfWeek != startOfWeek)
            {
                start = start.AddDays(-1);
            }

            DateTime end = start.AddDays(7);

            foreach (var request in requests)
            {
                if (request.CreatedAt >= start && request.CreatedAt < end)
                {
                    DayOfWeek dayOfWeek = request.CreatedAt.Value.DayOfWeek;
                    requestPerDay[dayOfWeek]++;
                }
            }

            return requestPerDay;
        }

        public List<GetListHiringRequest> GetRecentRequest()
        {
            IQueryable<HiringRequest> requests = _unitOfWork.RequestRepository.Get(r => r.Status != (int)HiringRequestStatus.Saved)
                                                           .Include(r => r.Company)
                                                           .OrderByDescending(n => n.CreatedAt);

            var mappedRequests = _mapper.Map<List<GetListHiringRequest>>(requests.Take(7));
            return mappedRequests;
        }      
    }
}
