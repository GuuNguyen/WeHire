using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Report
{
    public class CreateReportModel
    {
        public int? DeveloperId { get; set; }
        public int? ProjectId { get; set; }
        public int? ReportTypeId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportContent { get; set; }
    }
}
