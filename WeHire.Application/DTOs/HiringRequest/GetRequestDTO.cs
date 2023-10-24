﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class GetRequestDTO
    {
        public int RequestId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int NumberOfDev { get; set; }
        public int? TargetedDev { get; set; }
        public decimal SalaryPerDev { get; set; }
        public DateTime Duration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public string StatusString { get; set; }
        public bool? BookMark { get; set; }
        public int? CompanyId { get; set; }
    }
}
