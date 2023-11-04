using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Gender
    {
        public Gender()
        {
            Developers = new HashSet<Developer>();
        }

        public int GenderId { get; set; }
        public string GenderName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Developer> Developers { get; set; }
    }
}
