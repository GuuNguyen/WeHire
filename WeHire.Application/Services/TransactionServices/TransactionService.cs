using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Infrastructure.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.Services.TransactionServices
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


        public List<GetTransactionDTO> GetTransactions(PagingQuery query, SearchTransactionDTO searchKey)
        {
            IQueryable<Transaction> transactions = _unitOfWork.TransactionRepository.GetAll()
                                                              .Include(t => t.PayPeriod)
                                                              .Include(t => t.PayPeriod.Project)
                                                              .Include(t => t.PayPeriod.Project.Company)
                                                              .OrderByDescending(t => t.Timestamp);
            transactions = transactions.SearchItems(searchKey);
            transactions = transactions.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedTransaction = _mapper.Map<List<GetTransactionDTO>>(transactions);
            return mappedTransaction;
        }


        public async Task<List<GetTransactionDTO>> GetTransactionsByCompanyIdAsync(int companyId, PagingQuery query, SearchTransactionDTO searchKey)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var transactions = _unitOfWork.TransactionRepository.Get(t => t.Payer.CompanyPartners.FirstOrDefault().CompanyId == companyId)
                                                                .Include(t => t.PayPeriod)
                                                                .Include(t => t.PayPeriod.Project)
                                                                .OrderByDescending(t => t.Timestamp)
                                                                .AsQueryable();
            transactions = transactions.SearchItems(searchKey);
            transactions = transactions.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedTransaction = _mapper.Map<List<GetTransactionDTO>>(transactions);
            return mappedTransaction;
        }


        public async Task CreateTransactionAsync(Transaction requestBody)
        {
            var isHR = _unitOfWork.UserRepository.Get(u => u.UserId == requestBody.PayerId && u.RoleId == (int)RoleEnum.HR)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_ID_FIELD, ErrorMessage.NOT_ALLOWS);

            await _unitOfWork.TransactionRepository.InsertAsync(requestBody);    
            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<int> GetTotalItemAsync(int? companyId = null)
        {
            var transactions = _unitOfWork.TransactionRepository.GetAll()
                                                                .Include(t => t.Payer)
                                                                .ThenInclude(p => p.CompanyPartners)
                                                                .AsQueryable();

            if (companyId.HasValue)
            {
                transactions = transactions.Where(r => r.Payer.CompanyPartners.FirstOrDefault().CompanyId == companyId.Value);
            }

            var totalItemCount = await transactions.CountAsync();

            return totalItemCount;
        }
    }
}
