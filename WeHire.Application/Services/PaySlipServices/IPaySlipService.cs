using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PaySlip;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.PaySlipServices
{
    public interface IPaySlipService
    {
        public List<GetPaySlipModel> GetPaySlipsByPayPeriodId(int payPeriodId, PagingQuery query);
        public Task<GetUpdatePaySlipResponse> UpdateTotalOvertimeHourAsync(UpdatePaySlipModel requestBody);
    }
}
