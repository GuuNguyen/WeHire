using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.TransactionServices
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateTransactionAsync(Transaction requestBody)
        {
            var isHR = _unitOfWork.UserRepository.Get(u => u.UserId == requestBody.PayerId && u.RoleId == (int)RoleEnum.HR)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_ID_FIELD, ErrorMessage.NOT_ALLOWS);

            await _unitOfWork.TransactionRepository.InsertAsync(requestBody);    
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
