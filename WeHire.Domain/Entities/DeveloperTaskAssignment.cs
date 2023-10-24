using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class DeveloperTaskAssignment
    {
        public int DeveloperId { get; set; }
        public int TaskId { get; set; }
        public int Status { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual AssignTask Task { get; set; }
    }
}
