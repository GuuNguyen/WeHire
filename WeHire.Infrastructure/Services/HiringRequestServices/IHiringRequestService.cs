using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.HiringRequestServices
{
    public interface IHiringRequestService
    {
        public List<GetAllFieldRequest> GetAllRequest(PagingQuery query, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchSalary);
        public Task<GetAllFieldRequest> GetRequestByIdAsync(int requestId);
        public Task<GetRequestDTO> CreateRequestAsync(CreateRequestDTO requestBody);
        public Task<GetRequestDTO> UpdateRequestAsync(int requestId, UpdateRequestDTO requestBody);
        public Task<GetRequestDTO> CloneARequestAsync(int requestId);
        public List<GetAllFieldRequest> GetRequestsByDevId(int devId, int status);
        public Task<List<GetAllFieldRequest>> GetRequestsByCompanyId(int companyId, PagingQuery query, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchExtensionKey);
        public Task<int> GetTotalRequestsAsync(int? companyId = null);
        public Task CheckRequestDeadline(DateTime currentTime);
        public List<EnumDetailDTO> GetRequestStatus();
    }
}
