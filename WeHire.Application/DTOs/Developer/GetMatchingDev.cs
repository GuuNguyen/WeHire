using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class GetMatchingDev : MatchingPercentage
    {
        public int DeveloperId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CodeName { get; set; }
        public string UserImage { get; set; }
        public int? YearOfExperience { get; set; }
        public int? AverageSalary { get; set; }
        public int InterviewRound { get; set; }
        public string? DevStatusString { get; set; }
        public string? SelectedDevStatus { get; set; }
        public string LevelRequireName { get; set; }
        public List<string> TypeRequireStrings { get; set; }
        public List<string> SkillRequireStrings { get; set; }
    }
}
