using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Contract
{
    public class GetListContract
    {
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public string? DateSigned { get; set; }
        public string? CompanyPartnerName { get; set; }
        public string? HumanResourceName { get; set; }
        public string? DeveloperName { get; set; }
        public string? CreatedAt { get; set; }
        public string? StatusString { get; set; }
    }
}
