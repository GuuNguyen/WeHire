using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Agreement
    {
        public Agreement()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int AgreementId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? PaymentPeriod { get; set; }
        public decimal? CommissionRate { get; set; }
        public decimal? TotalCommission { get; set; }
        public string CompanyPartnerName { get; set; }
        public string ConfidentialInformation { get; set; }
        public string ServicesProvided { get; set; }
        public string CommissionStructure { get; set; }
        public string TermsAndConditions { get; set; }
        public DateTime? DateSigned { get; set; }
        public string WeHireSignature { get; set; }
        public string CompanyPartnerSignature { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Status { get; set; }
        public int? RequestId { get; set; }

        public virtual HiringRequest Request { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
