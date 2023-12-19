using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PaySlip
{
    public class GetPaySlipByDevModel
    {
        public int PaySlipId { get; set; }
        public string WorkForMonth { get; set; }
        public decimal? TotalActualWorkedHours { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public string TotalEarnings { get; set; }
        public string StatusString { get; set; }
    }
}
