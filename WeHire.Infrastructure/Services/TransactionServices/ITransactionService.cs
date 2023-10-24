using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Transaction;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.TransactionServices
{
    public interface ITransactionService
    {
        public Task CreateTransactionAsync(Transaction requestBody);
    }
}
