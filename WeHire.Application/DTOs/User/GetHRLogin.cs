﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.User
{
    public class GetHRLogin
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string DateOfBirth { get; set; }
        public string? UserImage { get; set; }
        public string? StatusString { get; set; }
        public string? RoleString { get; set; }
        public int? CompanyId { get; set; }
    }
}
