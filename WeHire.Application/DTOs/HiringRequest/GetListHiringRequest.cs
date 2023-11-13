using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class GetListHiringRequest
    {
        public int RequestId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyImage { get; set; }
        public string RequestCode { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? NumberOfDev { get; set; }
        public int? TargetedDev { get; set; }
        public decimal? SalaryPerDev { get; set; }
        public string Duration { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }
    }
}
