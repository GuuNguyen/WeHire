using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Agreement;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.AgreementEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;

namespace WeHire.Infrastructure.Services.AgreementServices
{
    public class AgreementService : IAgreementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AgreementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetAgreementDTO> CreateAgreementAsync(int requestId)
        {
            decimal commissionRate = 0.13m;
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId && r.Status == (int)HiringRequestStatus.InProgress)
                                                             .AsNoTracking()
                                                             .Include(r => r.Company)
                                                             .SingleOrDefaultAsync()         
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var newAgreement = new Agreement
            {   
                RequestId = requestId,
                CommissionRate = commissionRate,
                TotalCommission = CalculateTotalCommission(commissionRate, request.NumberOfDev, request.SalaryPerDev),
                CompanyPartnerName = request.Company.CompanyName,
                ConfidentialInformation = "The following services shall be provided by the party as agreed: " +
                                          "1. The Company Partner agrees to engage the WeHire Company to provide employees frothier business needs." +
                                          "2. The WeHire Company shall provide qualified employees to the Company Partner based on their specified requirements.",
                ServicesProvided = "",
                TermsAndConditions = "The terms of this contract are detailed as follows:" +
                                     "1. Terms" +
                                     "This Agreement shall commence on the Effective Date and shall remain in effect untilterminated by either party with 7 days written notice." +
                                     "The Agreement may be extended upon the provision of written consent from both Parties." +
                                     "Confidentiality" +
                                     "Both Parties shall maintain the confidentiality of all information shared during the course of thisAgreement and shall not disclose it to any third party except where required by law." +
                                     "Termination" +
                                     "This Agreement may be terminated:Immediately, in the event that one of the Parties breaches this Agreement." +
                                     "At any given time by providing written notice to the other party with the notice periodspecified in Term" +
                                     "Upon termination of this Agreement, the WeHire Company shall provide a final invoice, and theCompany Partner shall make any outstanding commission payments…" +
                                     "Representations and Warranties" +
                                     "Both Parties represent and warrant that they have the authority to enter into this Agreement and that their performance under this Agreement will not violate any third-party rights or any other agreement." +
                                     "Entire AgreementThis Agreement contains the entire agreement between the Parties and supersedes all prioragreements, understandings, or conditions." +
                                     "IN WITNESS WHEREOF, the Parties have executed this Commission Agreement as of theEffective Dat",
                CreateAt = DateTime.Now,
                Status = (int)AgreementStatus.PendingApproval,
            };

            await _unitOfWork.AgreementRepository.InsertAsync(newAgreement);
            await _unitOfWork.SaveChangesAsync();

            var mappedAgreement = _mapper.Map<GetAgreementDTO>(newAgreement);
            return mappedAgreement;
        }

        public Task<GetAgreementDTO> GetAgreementByIdAsync(int agreementId)
        {
            throw new NotImplementedException();
        }

        private static decimal CalculateTotalCommission(decimal rate, int? numberOfDev, decimal? salaryPerDev)
        {
            var total = rate * numberOfDev * salaryPerDev;
            return (decimal)total!;
        }

        public Task<GetAgreementPaper> GetAgreementPaperAsync(int agreementId)
        {
            throw new NotImplementedException();
        }
    }
}
