using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.CompanyPartner
{
    public class GetCompanyDetail
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? Rating { get; set; }
        public string CompanyImage { get; set; }
        public string Country { get; set; }
        public string StatusString { get; set; }
        public int? UserId { get; set; }
        public string? HRFullName { get; set; }
    }
}
