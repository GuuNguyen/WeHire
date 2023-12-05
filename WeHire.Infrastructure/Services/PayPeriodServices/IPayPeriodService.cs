using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PayPeriod;

namespace WeHire.Infrastructure.Services.PayPeriodServices
{
    public interface IPayPeriodService
    {
        public Task<List<PayPeriodInMonth>> GetPayPeriodsInProjectDurationAsync(int projectId);
        public DurationInMonth GetPayPeriodDuration(DateTime? startProjectDate, DateTime? endProjectDate, DateTime dateInput);
        public Task<ExcelFileModel> GeneratePayPeriodExcelFile(int projectId, DateTime inputDate);
        public Task<GetPayPeriodBill> GetPayPeriodByProject(int projectId, DateTime inputDate);
        public Task<string> InsertPayPeriodFromExcel(int projectId, ImportPaySlipModel importPaySlipModel);
        public Task<GetPayPeriodModel> CreatePayPeriod(CreatePayPeriodModel requestBody);
        public decimal? GetTotalEarningByDeveloperCode(int? hiredDeveloperId, decimal? totalActualWorkedHours, decimal? overtimeHours);
    }
}
