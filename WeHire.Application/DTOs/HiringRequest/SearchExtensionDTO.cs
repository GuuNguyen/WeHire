using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class SearchExtensionDTO
    {
        public List<int>? SkillIds { get; set; } 
        public decimal? StartSalaryPerDev { get; set; }
        public decimal? EndSalaryPerDev { get; set; }
    }
}
