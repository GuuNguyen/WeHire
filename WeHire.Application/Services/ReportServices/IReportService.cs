using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Report;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.ReportServices
{
    public interface IReportService
    {
        public List<GetReportModel> GetAllReport(SearchReportModel searchKey);
        public List<GetReportModel> GetReportByCompany(int companyId, SearchReportModel searchKey);
        public Task<GetReportModel> GetReportById(int reportId);
        public Task<GetReport> CreateReport(CreateReportModel requestModel);
        public Task<GetReport> ApproveReport(int  reportId);
        public Task<GetReport> ReplyReport(ReplyReport requestBody);
        public Task<int> GetTotalItem(int? projectId = null);
    }
}
