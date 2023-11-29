using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Project
{
    public class GetListProjectDTO
    {
        public int ProjectId { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyImage { get; set; }
        public string CompanyName { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int? NumberOfDev { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }
    }
}
