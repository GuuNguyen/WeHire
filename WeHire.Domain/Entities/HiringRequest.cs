using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class HiringRequest
    {
        public HiringRequest()
        {
            Interviews = new HashSet<Interview>();
            SelectedDevs = new HashSet<SelectedDev>();
            SkillRequires = new HashSet<SkillRequire>();
        }

        public int RequestId { get; set; }
        public string RequestCode { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int? NumberOfDev { get; set; }
        public int? TargetedDev { get; set; }
        public decimal? SalaryPerDev { get; set; }
        public DateTime? Duration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public bool? IsExpiredOnce { get; set; }
        public string RejectionReason { get; set; }
        public int Status { get; set; }
        public int? CompanyId { get; set; }
        public int? JobPositionId { get; set; }
        public int? TypeRequireId { get; set; }
        public int? LevelRequireId { get; set; }
        public int? EmploymentTypeId { get; set; }

        public virtual CompanyPartner Company { get; set; }
        public virtual EmploymentType EmploymentType { get; set; }
        public virtual JobPosition JobPosition { get; set; }
        public virtual Level LevelRequire { get; set; }
        public virtual Type TypeRequire { get; set; }
        public virtual ICollection<Interview> Interviews { get; set; }
        public virtual ICollection<SelectedDev> SelectedDevs { get; set; }
        public virtual ICollection<SkillRequire> SkillRequires { get; set; }
    }
}
