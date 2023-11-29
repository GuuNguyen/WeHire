using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class CompanyPartner
    {
        public CompanyPartner()
        {
            HiringRequests = new HashSet<HiringRequest>();
            Projects = new HashSet<Project>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string CompanyImage { get; set; }
        public string BackgroundImage { get; set; }
        public int? Rating { get; set; }
        public string FacebookUrl { get; set; }
        public string LinkedInkUrl { get; set; }
        public int Status { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
