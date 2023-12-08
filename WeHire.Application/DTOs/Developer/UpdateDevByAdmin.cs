using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Developer
{
    public class UpdateDevByAdmin : FileDTO
    {
        [Required(ErrorMessage = "DeveloperId is required")]
        public int DeveloperId { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "YearOfExperience is required")]
        public int? YearOfExperience { get; set; }
        [Required(ErrorMessage = "AverageSalary is required")]
        public decimal? AverageSalary { get; set; }
        [Required(ErrorMessage = "CodeName is required")]
        public string CodeName { get; set; }
        [Required(ErrorMessage = "Summary is required")]
        public string Summary { get; set; }
        [Required(ErrorMessage = "GenderId is required")]
        public int? GenderId { get; set; }
        [Required(ErrorMessage = "LevelId is required")]
        public int? LevelId { get; set; }
        [Required(ErrorMessage = "EmploymentTypeId is required")]
        public int? EmploymentTypeId { get; set; }
        [Required(ErrorMessage = "Types is required")]
        public List<int> Types { get; set; }
        [Required(ErrorMessage = "Skills is required")]
        public List<int> Skills { get; set; }
    }
}
