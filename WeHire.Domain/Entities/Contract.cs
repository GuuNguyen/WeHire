using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Contract
    {
        public int ContractId { get; set; }
        public DateTime? DateSigned { get; set; }
        public DateTime? StartWorkingDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public int? HiredDeveloperId { get; set; }

        public virtual HiredDeveloper HiredDeveloper { get; set; }
    }
}
