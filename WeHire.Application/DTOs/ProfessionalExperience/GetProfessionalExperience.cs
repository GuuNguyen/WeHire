﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.ProfessionalExperience
{
    public class GetProfessionalExperience
    {
        public int ProfessionalExperienceId { get; set; }
        public int? DeveloperId { get; set; }
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public string StartDateMMM { get; set; }
        public string EndDateMMM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}
