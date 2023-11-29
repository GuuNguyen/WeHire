using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Contract
{
    public class SearchContractDTO
    {
        public string? ContractCode { get; set; }
        public int? Status { get; set; }
    }
}
