using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Transaction
{
    public class CreateTransactionDTO
    {
        public int? PayerId { get; set; }
        public int? AgreementId { get; set; }
        public string? PayPalTransactionId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string? PaymentMethod { get; set; }
        public int Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? State { get; set; }
        public int Status { get; set; }
    }
}
