using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class JobPosition
    {
        public JobPosition()
        {
            HiredDevelopers = new HashSet<HiredDeveloper>();
            HiringRequests = new HashSet<HiringRequest>();
        }

        public int JobPositionId { get; set; }
        public string PositionName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<HiredDeveloper> HiredDevelopers { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
    }
}
