using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.HiringRequestServices
{
    public interface IHiringRequestService
    {
        public List<GetListHiringRequest> GetAllRequest(PagingQuery query, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchSalary);
        public Task<GetAllFieldRequest> GetRequestByIdAsync(int requestId);
        public Task<GetRequestDTO> CreateRequestAsync(CreateRequestDTO requestBody);
        public Task<GetRequestDTO> UpdateRequestAsync(int requestId, UpdateRequestDTO requestBody);
        public Task<GetRequestDTO> CloneARequestAsync(int requestId);
        public List<GetAllFieldRequest> GetRequestsByDevId(int devId, int status, string? searchKeyString, SearchHiringRequestDTO searchKey);
        public Task<List<GetAllFieldRequest>> GetRequestsByCompanyId(int companyId, PagingQuery query, string? searchKeyString, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchExtensionKey);
        public Task<List<GetAllFieldRequest>> GetRequestsByProjectId(int projectId, PagingQuery query, string? searchKeyString, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchExtensionKey);
        public Task HandleDeveloperAfterCloseHiringRequest(HiringRequest request);
        public Task<int> GetTotalRequestsAsync(int? companyId = null);
        public Task CheckRequestDeadline(DateTime currentTime);
        public Task CheckRequestExpired(DateTime currentTime);
        public Task<int> GetTotalRequestsByProjectIdAsync(int projectId);
        public List<EnumDetailDTO> GetRequestStatus();
        public Task<string> DeleteHiringRequest(int requestId);
    }
}
