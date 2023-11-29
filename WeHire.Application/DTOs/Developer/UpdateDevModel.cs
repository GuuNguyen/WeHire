using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.Developer
{
    public class UpdateDevModel : FileDTO
    {
        [Required]
        public int DeveloperId { get; set; }
        [Required]
        public int? GenderId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string Summary { get; set; }
    }
}
