using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Transaction
{
    public class GetTransactionDTO
    {
        public int TransactionId { get; set; }
        public int? PayerId { get; set; }
        public int? PayPeriodId { get; set; }
        public string? PayPalTransactionId { get; set; }
        public string Timestamp { get; set; }
        public string? PaymentMethod { get; set; }
        public string Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? State { get; set; }
        public string? StatusString { get; set; }
    }
}
