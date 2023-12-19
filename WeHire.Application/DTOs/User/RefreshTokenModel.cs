using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.User
{
    public class RefreshTokenModel
    {
        public DateTime AccessTokenExp { get; set; }
        public DateTime RefreshTokenExp { get; set; }
        [Required(ErrorMessage = "AccessToken is required")]
        public string AccessToken { get; set; }
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; }
    }
}
