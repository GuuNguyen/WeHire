using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.User
{
    public class UserLoginResponse
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public DateTime AccessTokenExp { get; set; }
        public DateTime RefreshTokenExp { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
