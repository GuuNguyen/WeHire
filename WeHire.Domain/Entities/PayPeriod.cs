using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class PayPeriod
    {
        public PayPeriod()
        {
            PaySlips = new HashSet<PaySlip>();
            Transactions = new HashSet<Transaction>();
        }

        public int PayPeriodId { get; set; }
        public string PayPeriodCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<PaySlip> PaySlips { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
