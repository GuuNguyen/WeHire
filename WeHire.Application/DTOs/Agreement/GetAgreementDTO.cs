using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Agreement
{
    public class GetAgreementDTO
    {
        public int AgreementId { get; set; }
        public int? RequestId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? PaymentPeriod { get; set; }
        public decimal? CommissionRate { get; set; }
        public decimal? TotalCommission { get; set; }
        public string CompanyPartnerName { get; set; }
        public DateTime? DateSigned { get; set; }
        public string WeHireSignature { get; set; }
        public string CompanyPartnerSignature { get; set; }
        public DateTime? CreateAt { get; set; }
        public string StatusString { get; set; }
    }
}
