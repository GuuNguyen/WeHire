using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.Report;
using WeHire.Application.Services.NotificationServices;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Infrastructure.IRepositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.ReportEnum;

namespace WeHire.Application.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        public ReportService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public List<GetReportModel> GetAllReport(SearchReportModel searchKey)
        {
            var reports = _unitOfWork.ReportRepository.GetAll()
                                                      .Include(r => r.ReportType)
                                                      .Include(r => r.Project)
                                                      .Include(r => r.HiredDeveloper)
                                                      .Include(r => r.HiredDeveloper.Developer)
                                                      .Include(r => r.HiredDeveloper.Developer.User)
                                                      .OrderByDescending(r => r.CreateAt)
                                                      .AsQueryable();

            reports = reports.SearchItems(searchKey);

            var mappedReports = _mapper.Map<List<GetReportModel>>(reports);
            return mappedReports;
        }


        public List<GetReportModel> GetReportByCompany(int companyId, SearchReportModel searchKey)
        {
            var reports = _unitOfWork.ReportRepository.Get(r => r.Project.CompanyId == companyId)
                                                      .Include(r => r.ReportType)
                                                      .Include(r => r.Project)
                                                      .Include(r => r.HiredDeveloper)
                                                      .Include(r => r.HiredDeveloper.Developer)
                                                      .Include(r => r.HiredDeveloper.Developer.User)
                                                      .OrderByDescending(r => r.CreateAt)
                                                      .AsQueryable();

            reports = reports.SearchItems(searchKey);

            var mappedReports = _mapper.Map<List<GetReportModel>>(reports);
            return mappedReports;
        }


        public async Task<GetReportModel> GetReportById(int reportId)
        {
            var report = await _unitOfWork.ReportRepository.Get(r => r.ReportId == reportId)
                                                      .Include(r => r.ReportType)
                                                      .Include(r => r.Project)
                                                      .Include(r => r.HiredDeveloper)
                                                      .Include(r => r.HiredDeveloper.Developer)
                                                      .Include(r => r.HiredDeveloper.Developer.User)
                                                      .SingleOrDefaultAsync()
            ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Report", "Report does not exist!!");

            var developer = await _unitOfWork.HiredDeveloperRepository.Get(h => h.HiredDeveloperId == report.HiredDeveloperId)
                                                                    .Include(h => h.Developer.HiredDevelopers)
                                                                    .ThenInclude(h => h.Contract)
                                                                    .Include(s => s.Developer.User)
                                                                    .Include(s => s.Developer.Gender)
                                                                    .Include(r => r.Developer.EmploymentType)
                                                                    .Include(s => s.Developer.Level)
                                                                    .Include(s => s.Developer.DeveloperTypes)
                                                                       .ThenInclude(dt => dt.Type)
                                                                    .Include(s => s.Developer.DeveloperSkills)
                                                                       .ThenInclude(ds => ds.Skill)
                                                                    .SingleOrDefaultAsync();
            var mappedReport = _mapper.Map<GetReportModel>(report);
            mappedReport.DeveloperInProject = _mapper.Map<GetDeveloperInProject>(developer);
            return mappedReport;
        }


        public async Task<GetReport> ApproveReport(int reportId)
        {
            var report = await _unitOfWork.ReportRepository.Get(r => r.ReportId == reportId &&
                                                                      r.Status == (int)ReportStatus.Pending)
                                                            .SingleOrDefaultAsync()
             ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Report", "Report does not exist!!");
            report.Status = (int)ReportStatus.Processing;
            await _unitOfWork.SaveChangesAsync();
            
            var mappedReports = _mapper.Map<GetReport>(report);
            return mappedReports;
        }

        public async Task<GetReport> CreateReport(CreateReportModel requestModel)
        {
            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.DeveloperId ==  requestModel.DeveloperId &&
                                                                                h.ProjectId == requestModel.ProjectId &&
                                                                                h.Status == (int)HiredDeveloperStatus.Working)
                                                                           .Include(h => h.Project)
                                                                           .ThenInclude(p => p.Company)
                                                                           .Include(h => h.Developer)
                                                                           .ThenInclude(d => d.User)
                                                                           .SingleOrDefaultAsync()
            ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper does not exist!!");

            var newReport = _mapper.Map<Domain.Entities.Report>(requestModel);

            newReport.HiredDeveloper = hiredDeveloper;
            newReport.CreateAt = DateTime.Now;
            newReport.Status = (int)ReportStatus.Pending;

            await _unitOfWork.ReportRepository.InsertAsync(newReport);
            await _unitOfWork.SaveChangesAsync();

            await _notificationService.SendManagerNotificationAsync(hiredDeveloper.Project.Company.CompanyName, newReport.ReportId, NotificationTypeString.REPORT,
            $"{hiredDeveloper.Project.Company.CompanyName} are posted a new report about developer {hiredDeveloper.Developer.User.FirstName} {hiredDeveloper.Developer.User.LastName}.");
            
            var mappedReports = _mapper.Map<GetReport>(newReport);
            return mappedReports;
        }

        public async Task<GetReport> ReplyReport(ReplyReport requestBody)
        {
            var report = await _unitOfWork.ReportRepository.Get(r => r.ReportId == requestBody.ReportId && 
                                                                     r.Status == (int)ReportStatus.Processing)
                                                           .SingleOrDefaultAsync()
            ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Report", "Report does not exist!!");

            report.ResponseContent = requestBody.ResponseContent;
            report.Status = (int)ReportStatus.Done;

            await _unitOfWork.SaveChangesAsync();

            var mappedReports = _mapper.Map<GetReport>(report);
            return mappedReports;
        }

        public async Task<int> GetTotalItem(int? companyId = null)
        {
            var query = _unitOfWork.ReportRepository.GetAll().Include(r => r.Project);
            var total = query.Count();
            if (companyId.HasValue)
                return total = await query.Where(r => r.Project.CompanyId == companyId).CountAsync();
            return total;
        }     
    }
}
