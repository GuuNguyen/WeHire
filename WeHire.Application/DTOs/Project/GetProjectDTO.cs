﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Project
{
    public class GetProjectDTO
    {
        public int ProjectId { get; set; }
        public int? CompanyId { get; set; }
        public int? ProjectTypeId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int? NumberOfDev { get; set; }
        public string BackgroundImage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusString { get; set; }
    }
}
