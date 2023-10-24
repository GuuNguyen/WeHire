﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class CreateRequestDTO
    {
        public string JobTitle { get; set; }
        public string? JobDescription { get; set; }
        public int? NumberOfDev { get; set; }
        public decimal? SalaryPerDev { get; set; }
        public DateTime? Duration { get; set; }
        public int? CompanyId { get; set; }

        public int? ScheduleTypeId { get; set; }
        public int? EmploymentTypeId { get; set; }

        public int? TypeRequireId { get; set; }
        public int? LevelRequireId { get; set; }
        public List<int>? SkillIds { get; set; }

        public bool isSaved { get; set; } = false;
    }
}
