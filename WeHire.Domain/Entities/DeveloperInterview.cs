using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class DeveloperInterview
    {
        public int InterviewId { get; set; }
        public int DeveloperId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual Interview Interview { get; set; }
    }
}
