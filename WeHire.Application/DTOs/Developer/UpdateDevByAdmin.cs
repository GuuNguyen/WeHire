using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.Developer
{
    public class UpdateDevByAdmin : FileDTO
    {
        public int DeveloperId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? YearOfExperience { get; set; }
        public decimal? AverageSalary { get; set; }
        public string CodeName { get; set; }
        public string Summary { get; set; }
        public int? GenderId { get; set; }
        public int? LevelId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public List<int> Types { get; set; }
        public List<int> Skills { get; set; }
    }
}
