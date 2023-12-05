using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.ReportType
{
    public class GetReportTypeModel
    {
        public int ReportTypeId { get; set; }
        public string ReportTypeTitle { get; set; }
        public int Status { get; set; }
    }
}
