using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Agreement
{
    public class CreateAgreementDTO
    {
        public int? RequestId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? PaymentPeriod { get; set; }
        public string Service { get; set; }
        public string Term { get; set; }
        public string Confidentiality { get; set; }
        public string Termination { get; set; }
        public string RepresentationsAndWarranties { get; set; }
        public string GoverningLaw { get; set; }
        public string EntireAgreement { get; set; }
    }
}
