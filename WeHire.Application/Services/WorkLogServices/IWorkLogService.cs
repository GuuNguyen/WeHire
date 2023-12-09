using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.WorkLog;

namespace WeHire.Application.Services.WorkLogServices
{
    public interface IWorkLogService
    {
        public Task<List<GetWorkLogModel>> GetWorkLogByPaySlipIdAsync(int paySlipId);
        public Task<WorkLogResponseModel> UpdateWorkLogAsync(UpdateWorkLogModel requestBody);
    }
}
