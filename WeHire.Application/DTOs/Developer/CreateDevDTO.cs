using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using WeHire.Domain.Entities;

namespace WeHire.Application.DTOs.Developer
{
    public class CreateDevDTO 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? GenderId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? YearOfExperience { get; set; }
        public int? AverageSalary { get; set; }
        public int? Cvid { get; set; }
        public int? ScheduleTypeId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? LevelId { get; set; }
        public List<int> Types { get; set; }
        public List<int> Skills { get; set; }
    }
}
