using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class ScheduleType
    {
        public ScheduleType()
        {
            Developers = new HashSet<Developer>();
            HiringRequests = new HashSet<HiringRequest>();
        }

        public int ScheduleTypeId { get; set; }
        public string ScheduleTypeName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
    }
}
