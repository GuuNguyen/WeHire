using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public string PayPalTransactionId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public int Status { get; set; }
        public int? PayerId { get; set; }
        public int? AgreementId { get; set; }

        public virtual Agreement Agreement { get; set; }
        public virtual User Payer { get; set; }
    }
}
