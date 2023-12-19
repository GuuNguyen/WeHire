using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.CompanyPartner
{
    public class CreateCompanyDTO : FileDTO
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "CompanyEmail is required")]
        public string CompanyEmail { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        [RegularExpression(@"\b((\+84|0)[0-9]{9,10})\b", ErrorMessage = "Phone number is invalid")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }
}
