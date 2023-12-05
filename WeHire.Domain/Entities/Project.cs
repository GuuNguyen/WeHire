﻿using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Project
    {
        public Project()
        {
            HiredDevelopers = new HashSet<HiredDeveloper>();
            HiringRequests = new HashSet<HiringRequest>();
            PayPeriods = new HashSet<PayPeriod>();
            Reports = new HashSet<Report>();
        }

        public int ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int? NumberOfDev { get; set; }
        public string BackgroundImage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public int? CompanyId { get; set; }
        public int? ProjectTypeId { get; set; }

        public virtual CompanyPartner Company { get; set; }
        public virtual ProjectType ProjectType { get; set; }
        public virtual ICollection<HiredDeveloper> HiredDevelopers { get; set; }
        public virtual ICollection<HiringRequest> HiringRequests { get; set; }
        public virtual ICollection<PayPeriod> PayPeriods { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
