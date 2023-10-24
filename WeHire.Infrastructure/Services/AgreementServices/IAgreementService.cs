using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Agreement;

namespace WeHire.Infrastructure.Services.AgreementServices
{
    public interface IAgreementService
    {
        public Task<GetAgreementDTO> GetAgreementByIdAsync(int agreementId);
        public Task<GetAgreementPaper> GetAgreementPaperAsync(int agreementId);
        public Task<GetAgreementDTO> CreateAgreementAsync(int requestId);
    }
}
