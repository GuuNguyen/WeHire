using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class SelectedDev
    {
        public int RequestId { get; set; }
        public int DeveloperId { get; set; }
        public int Status { get; set; }
        public int? InterviewId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual Interview Interview { get; set; }
        public virtual HiringRequest Request { get; set; }
    }
}
