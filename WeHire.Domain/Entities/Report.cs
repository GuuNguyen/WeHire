using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Report
    {
        public int ReportId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportContent { get; set; }
        public string ResponseContent { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Status { get; set; }
        public int? ReportTypeId { get; set; }
        public int? HiredDeveloperId { get; set; }
        public int? ProjectId { get; set; }

        public virtual HiredDeveloper HiredDeveloper { get; set; }
        public virtual Project Project { get; set; }
        public virtual ReportType ReportType { get; set; }
    }
}
