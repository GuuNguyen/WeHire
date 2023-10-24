using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.CompanyPartner
{
    public class CreateCompanyDTO : FileDTO
    {
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public int? UserId { get; set; }
    }
}
