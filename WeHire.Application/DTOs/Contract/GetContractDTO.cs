using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Contract
{
    public class GetContractDTO
    {
        public int ContractId { get; set; }
        public string ContractCode { get; set; }
        public DateTime? DateSigned { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public decimal? BasicSalary { get; set; }
        public int? StandardMonthlyWorkingHours { get; set; }
        public decimal? OvertimePayMultiplier { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusString { get; set; }      
    }
}
