﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.User
{
    public class CreateUserDTO 
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; } = null!;
    }
}
