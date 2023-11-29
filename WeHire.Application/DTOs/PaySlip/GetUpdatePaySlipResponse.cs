using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PaySlip
{
    public class GetUpdatePaySlipResponse
    {
        public decimal? TotalOvertimeHours { get; set; }
        public string TotalEarnings { get; set; }
    }
}
