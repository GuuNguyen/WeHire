using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int? ReportTypeId { get; set; }
        public int? CompanyId { get; set; }

        public virtual CompanyPartner Company { get; set; }
        public virtual ReportType ReportType { get; set; }
    }
}
