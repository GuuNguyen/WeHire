using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.ComapnyPartnerServices
{
    public interface ICompanyService
    {
        public List<GetCompanyDetail> GetCompany(PagingQuery query, SearchCompanyDTO searchKey);
        public Task<GetCompanyDetail> GetCompanyById(int id);
        public Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO requestBody);
        public Task<GetCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO requestBody);
        public Task DeleteCompanyAsync(int companyId);
        public Task<int> GetTotalItemAsync();
    }
}
