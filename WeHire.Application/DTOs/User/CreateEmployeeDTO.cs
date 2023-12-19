using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.User
{
    public class CreateEmployeeDTO
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"\b((\+84|0)[0-9]{9,10})\b", ErrorMessage = "Phone number is invalid")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "RoleId is required")]
        public int? RoleId { get; set; }
    }
}
