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
        public List<GetListInterview> GetInterviewsByManager(PagingQuery query, int? companyId, SearchInterviewWithRequest searchKey);
        public Task<GetInterviewWithDev> GetInterviewById(int interviewId);  
        public Task<List<GetListInterview>> GetInterviewByRequestIdAsync(int requestId);
        public Task<List<GetListInterview>> GetInterviewByDevId(int devId, SearchInterviewDTO searchKey);
        public Task<GetInterviewDTO> CreateInterviewAsync(CreateInterviewDTO requestBody);
        public Task<GetInterviewDTO> UpdateInterviewAsync(int interviewId, UpdateInterviewModel requestBody);
        public Task<GetInterviewDTO> ChangeStatusAsync(ChangeStatusDTO requestBody);
        public Task<GetInterviewDTO> CancelInterviewAsync(int interviewId);
        public Task ExpireInterviewAsync(DateTime currentTime);
        public Task<GetInterviewDTO> FinishInterviewAsync(int interviewId);
        public Task<int> GetTotalInterviewsAsync(int? companyId = null);
    }
}
