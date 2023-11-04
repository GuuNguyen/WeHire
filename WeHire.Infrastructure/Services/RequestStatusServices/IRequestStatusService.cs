using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using static WeHire.Application.DTOs.HiringRequest.ChangeStatusDTO;

namespace WeHire.Infrastructure.Services.RequestStatusServices
{
    public interface IRequestStatusService
    {
        public Task<GetRequestDTO> HandleWaitingStatusAsync(WaitingStatus requestBody);
        public Task<GetRequestDTO> CancelRequestAsync(CancelRequestModel requestBody);
        public Task<GetRequestDTO> HandleExpiredStatusAsync(ExpiredStatus requestBody);
    }
}
