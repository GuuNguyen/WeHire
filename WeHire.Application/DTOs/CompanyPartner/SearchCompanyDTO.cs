using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.CompanyPartner
{
    public class SearchCompanyDTO
    {
        public string CompanyName { get; set; } = string.Empty;
        public int? Status { get; set; } = null;
    }
}
