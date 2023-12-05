using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class HiredDeveloper
    {
        public HiredDeveloper()
        {
            Interviews = new HashSet<Interview>();
            PaySlips = new HashSet<PaySlip>();
            Reports = new HashSet<Report>();
        }

        public int HiredDeveloperId { get; set; }
        public int Status { get; set; }
        public int? ProjectId { get; set; }
        public int? RequestId { get; set; }
        public int? ContractId { get; set; }
        public int? DeveloperId { get; set; }

        public virtual Contract Contract { get; set; }
        public virtual Developer Developer { get; set; }
        public virtual Project Project { get; set; }
        public virtual HiringRequest Request { get; set; }
        public virtual ICollection<Interview> Interviews { get; set; }
        public virtual ICollection<PaySlip> PaySlips { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
