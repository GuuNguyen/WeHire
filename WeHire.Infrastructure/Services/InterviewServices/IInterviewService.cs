using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Interview;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.InterviewServices
{
    public interface IInterviewService
    {
        public List<GetInterviewDetail> GetInterviewsByCompany(int? companyId, int? requestId, PagingQuery query, SearchInterviewDTO searchKey);
        public List<GetInterviewDetail> GetInterviewsByManager(PagingQuery query, int? companyId, SearchInterviewWithRequest searchKey);
        public Task<GetInterviewWithDev> GetInterviewById(int interviewId);  
        public Task<List<GetInterviewDetail>> GetInterviewByRequestIdAsync(int requestId);
        public Task<List<GetInterviewDetail>> GetInterviewByDevId(int devId);
        public Task<GetInterviewDTO> CreateInterviewAsync(CreateInterviewDTO requestBody);
        public Task<GetInterviewDTO> ChangeStatusAsync(ChangeStatusDTO requestBody);
        public Task<int> GetTotalInterviewsAsync(int? companyId = null);

    }
}
