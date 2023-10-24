using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class EmploymentType
    {
        public EmploymentType()
        {
            Developers = new HashSet<Developer>();
            HiringRequests = new HashSet<HiringRequest>();
        }

        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
    }
}
