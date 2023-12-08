using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using WeHire.Domain.Entities;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Developer
{
    public class CreateDevDTO 
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "GenderId is required")]
        public int? GenderId { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "YearOfExperience is required")]
        public int? YearOfExperience { get; set; }
        [Required(ErrorMessage = "AverageSalary is required")]
        public int? AverageSalary { get; set; }
        [Required(ErrorMessage = "EmploymentTypeId is required")]
        public int? EmploymentTypeId { get; set; }
        [Required(ErrorMessage = "LevelId is required")]
        public int? LevelId { get; set; }
        [Required(ErrorMessage = "Types is required")]
        public List<int> Types { get; set; }
        [Required(ErrorMessage = "Skills is required")]
        public List<int> Skills { get; set; }
    }
}
