using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class ProjectType
    {
        public ProjectType()
        {
            Projects = new HashSet<Project>();
        }

        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
