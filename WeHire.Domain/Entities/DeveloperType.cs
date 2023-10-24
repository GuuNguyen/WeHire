using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class DeveloperType
    {
        public int DeveloperId { get; set; }
        public int TypeId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual Type Type { get; set; }
    }
}
