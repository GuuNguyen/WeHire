using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Education
    {
        public int EducationId { get; set; }
        public string MajorName { get; set; }
        public string SchoolName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public int? DeveloperId { get; set; }

        public virtual Developer Developer { get; set; }
    }
}
