using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CompanyPartner;

namespace WeHire.Infrastructure.Services.ComapnyPartnerServices
{
    public interface ICompanyService
    {
        public Task<GetCompanyDetail> GetCompanyById(int id);
        public Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO requestBody);
        public Task<GetCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO requestBody);
    }
}
