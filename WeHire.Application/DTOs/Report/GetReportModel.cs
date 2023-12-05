using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Report
{
    public class GetReportModel
    {
        public int ReportId { get; set; }
        public int HiredDeveloperId { get; set; }
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public int? DeveloperId { get; set; }
        public string DeveloperName { get; set; }
        public string DeveloperCode { get; set; }
        public string ReportTitle { get; set; }
        public string ReportTypeTitle { get; set; }
        public string ReportContent { get; set; }
        public string ResponseContent { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }
    }
}
