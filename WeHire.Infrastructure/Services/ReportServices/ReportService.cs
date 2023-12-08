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
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.ReportEnum;

namespace WeHire.Infrastructure.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetReportModel> GetAllReport(PagingQuery query, SearchReportModel searchKey)
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

            reports = reports.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedReports = _mapper.Map<List<GetReportModel>>(reports);
            return mappedReports;
        }


        public List<GetReportModel> GetReportByProject(int companyId, PagingQuery query, SearchReportModel searchKey)
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

            reports = reports.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

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
                                                                    .Select(s => s.Developer)
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
                                                                     .SingleOrDefaultAsync()
            ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper does not exist!!");

            var newReport = _mapper.Map<Domain.Entities.Report>(requestModel);

            newReport.HiredDeveloper = hiredDeveloper;
            newReport.CreateAt = DateTime.Now;
            newReport.Status = (int)ReportStatus.Pending;

            await _unitOfWork.ReportRepository.InsertAsync(newReport);
            await _unitOfWork.SaveChangesAsync();

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
