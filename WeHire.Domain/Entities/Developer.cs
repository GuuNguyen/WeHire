using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Developer
    {
        public Developer()
        {
            DeveloperSkills = new HashSet<DeveloperSkill>();
            DeveloperTypes = new HashSet<DeveloperType>();
            Educations = new HashSet<Education>();
            HiredDevelopers = new HashSet<HiredDeveloper>();
            ProfessionalExperiences = new HashSet<ProfessionalExperience>();
        }

        public int DeveloperId { get; set; }
        public int? YearOfExperience { get; set; }
        public decimal? AverageSalary { get; set; }
        public string CodeName { get; set; }
        public string Summary { get; set; }
        public int Status { get; set; }
        public int? UserId { get; set; }
        public int? LevelId { get; set; }
        public int? GenderId { get; set; }
        public int? EmploymentTypeId { get; set; }

        public virtual EmploymentType EmploymentType { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual Level Level { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<DeveloperSkill> DeveloperSkills { get; set; }
        public virtual ICollection<DeveloperType> DeveloperTypes { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<HiredDeveloper> HiredDevelopers { get; set; }
        public virtual ICollection<ProfessionalExperience> ProfessionalExperiences { get; set; }
    }
}
