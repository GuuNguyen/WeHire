using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.EmploymentType;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.Type;
using WeHire.Domain.Entities;

namespace WeHire.Application.DTOs.Developer
{
    public class GetDevDetail
    {
        public int UserId { get; set; }
        public int DeveloperId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CodeName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string UserImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Summary { get; set; }
        public int? YearOfExperience { get; set; }
        public int? AverageSalary { get; set; }
        public string ScheduleTypeName { get; set; }
        public string EmploymentTypeName { get; set; }
        public string? DevStatusString { get; set; }
        public string? UserStatusString { get; set; }
        public string GenderName { get; set; }
        public string Src { get; set; }

        public GetLevelDetail Level { get; set; }
        public List<GetSkillDetail> Skills { get; set; }
        public List<GetTypeDetail> Types { get; set; }
    }
}
