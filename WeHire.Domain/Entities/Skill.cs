using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Skill
    {
        public Skill()
        {
            DeveloperSkills = new HashSet<DeveloperSkill>();
            SkillRequires = new HashSet<SkillRequire>();
        }

        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string SkillDescription { get; set; }
        public int Status { get; set; }

        public virtual ICollection<DeveloperSkill> DeveloperSkills { get; set; }
        public virtual ICollection<SkillRequire> SkillRequires { get; set; }
    }
}
