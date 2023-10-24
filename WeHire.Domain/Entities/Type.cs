using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Type
    {
        public Type()
        {
            DeveloperTypes = new HashSet<DeveloperType>();
            HiringRequests = new HashSet<HiringRequest>();
        }

        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public int Status { get; set; }

        public virtual ICollection<DeveloperType> DeveloperTypes { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
    }
}
