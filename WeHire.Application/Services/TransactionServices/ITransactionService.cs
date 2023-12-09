using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;

namespace WeHire.Application.Services.TransactionServices
{
    public interface ITransactionService
    {
        public List<GetTransactionDTO> GetTransactions(PagingQuery query, SearchTransactionDTO searchKey);
        public Task<List<GetTransactionDTO>> GetTransactionsByCompanyIdAsync(int companyId, PagingQuery query, SearchTransactionDTO searchKey);
        public Task CreateTransactionAsync(Transaction requestBody);
        public Task<int> GetTotalItemAsync(int? companyId = null);
    }
}
