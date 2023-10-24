using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class DeveloperSkill
    {
        public int DeveloperId { get; set; }
        public int SkillId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
