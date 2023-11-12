using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class HiredDeveloper
    {
        public HiredDeveloper()
        {
            Contracts = new HashSet<Contract>();
            WorkLogs = new HashSet<WorkLog>();
        }

        public int HiredDeveloperId { get; set; }
        public decimal? Salary { get; set; }
        public int Status { get; set; }
        public int? ProjectId { get; set; }
        public int? DeveloperId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Contract> Contracts { get; set; }
        public virtual ICollection<WorkLog> WorkLogs { get; set; }
    }
}
