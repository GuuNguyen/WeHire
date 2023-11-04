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
