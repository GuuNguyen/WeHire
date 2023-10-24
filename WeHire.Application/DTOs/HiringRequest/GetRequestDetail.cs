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
    public class GetRequestDetail
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
        public bool? BookMark { get; set; }
        public GetCompanyDTO Company { get; set; }
        public GetTypeDetail TypeRequire { get; set; }
        public GetLevelDetail LevelRequire { get; set; }
        public List<GetSkillDetail> SkillDetails { get; set; }
    }
}
