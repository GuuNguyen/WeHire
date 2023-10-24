using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class SkillRequire
    {
        public int RequestId { get; set; }
        public int SkillId { get; set; }

        public virtual HiringRequest Request { get; set; }
        public virtual Skill Skill { get; set; }
    }
}
