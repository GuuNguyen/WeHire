using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.Type;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class GetAllFieldRequest
    {
        public int RequestId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int NumberOfDev { get; set; }
        public int? TargetedDev { get; set; }
        public decimal SalaryPerDev { get; set; }
        public DateTime Duration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string StatusString { get; set; }
        public string PostedTime { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyImage { get; set; }
        public string? ScheduleTypeName { get; set; }
        public string? EmploymentTypeName { get; set; }
        public bool? BookMark { get; set; }
        public string TypeRequireName { get; set; }
        public string LevelRequireName { get; set; }
        public List<string> SkillRequireStrings { get; set; }
    }
}
