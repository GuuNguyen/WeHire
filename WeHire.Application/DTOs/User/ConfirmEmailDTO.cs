using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.User
{
    public class ConfirmEmailDTO
    {
        public int UserId { get; set; }
        public string ConfirmationCode { get; set; }
    }
}
