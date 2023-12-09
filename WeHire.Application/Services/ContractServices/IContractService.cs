using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.ContractServices
{
    public interface IContractService
    {
        public List<GetListContract> GetContractAsync(PagingQuery query, SearchContractDTO searchKey);
        public List<GetListContract> GetContractByCompanyAsync(int companyId, PagingQuery query, SearchContractDTO searchKey);
        public Task<GetPreContract> GetPreContractAsync(int developerId, int requestId);
        public Task<GetContractDetail> GetContractByIdAsync(int contractId);
        public Task<GetContractDetail> GetContractByDevAsync(int developerId, int projectId);
        public Task<GetContractDTO> CreateContractAsync(CreateContractDTO requestBody);
        public Task<GetContractDTO> ConfirmSignedContractAsync(int contractId);
        public Task<GetContractDTO> FailContractAsync(int contractId);
        public Task FailContractOnBackgroundAsync(DateTime currentDate);
        public Task<int> GetTotalItemAsync(int? companyId = null);
    }
}
