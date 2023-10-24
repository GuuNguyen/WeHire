using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Enums;

namespace WeHire.Application.DTOs.User
{
    public class ChangeRoleDTO
    {
        public int UserId { get; set; }
        public int Role { get; set; }
    }
}
