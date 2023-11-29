using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class HiredDeveloper
    {
        public HiredDeveloper()
        {
            PaySlips = new HashSet<PaySlip>();
        }

        public int HiredDeveloperId { get; set; }
        public int Status { get; set; }
        public int? JobPositionId { get; set; }
        public int? ProjectId { get; set; }
        public int? ContractId { get; set; }
        public int? DeveloperId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Developer Developer { get; set; }
        public virtual JobPosition JobPosition { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<PaySlip> PaySlips { get; set; }
    }
}
