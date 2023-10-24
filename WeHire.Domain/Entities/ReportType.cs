using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class ReportType
    {
        public ReportType()
        {
            Reports = new HashSet<Report>();
        }

        public int ReportTypeId { get; set; }
        public string ReportTypeTitle { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}
