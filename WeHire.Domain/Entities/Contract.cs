using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Contract
    {
        public Contract()
        {
            HiredDevelopers = new HashSet<HiredDeveloper>();
        }

        public int ContractId { get; set; }
        public string ContractCode { get; set; }
        public string LegalRepresentation { get; set; }
        public string LegalRepresentationPosition { get; set; }
        public DateTime? DateSigned { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? BasicSalary { get; set; }
        public string EmployementType { get; set; }
        public int? StandardMonthlyWorkingHours { get; set; }
        public decimal? OvertimePayMultiplier { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }

        public virtual ICollection<HiredDeveloper> HiredDevelopers { get; set; }
    }
}
