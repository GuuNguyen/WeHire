using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Level
    {
        public Level()
        {
            Developers = new HashSet<Developer>();
            HiringRequests = new HashSet<HiringRequest>();
        }

        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
    }
}
