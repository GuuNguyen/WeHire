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
using WeHire.Entity.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.PayPeriodEnum;
using static WeHire.Domain.Enums.ProjectEnum;
using static WeHire.Domain.Enums.TransactionEnum;

namespace WeHire.Infrastructure.Services.DashboardServices
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
            var transaction = _unitOfWork.TransactionRepository.Get(t => t.Status == (int)TransactionStatus.Success).ToList();

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
