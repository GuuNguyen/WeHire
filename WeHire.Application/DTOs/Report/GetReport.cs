using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Report
{
    public class GetReport
    {
        public int ReportId { get; set; }
        public int? ReportTypeId { get; set; }
        public int? HiredDeveloperId { get; set; }
        public int? ProjectId { get; set; }
        public string ReportTitle { get; set; }
        public string ReportContent { get; set; }
        public string ResponseContent { get; set; }
        public string CreateAt { get; set; }
        public string StatusString { get; set; }
    }
}
