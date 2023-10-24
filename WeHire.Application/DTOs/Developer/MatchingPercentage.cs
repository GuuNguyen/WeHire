using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class MatchingPercentage
    {
        public bool TypeMatching { get; set; }
        public bool LevelMatching { get; set; }
        public double SalaryPerDevPercentage { get; set; }
        public double SkillPercentage { get; set; }
        public double AveragedPercentage { get; set; }
    }
}
