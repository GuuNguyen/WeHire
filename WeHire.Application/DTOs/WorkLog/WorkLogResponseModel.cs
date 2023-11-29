using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.WorkLog
{
    public class WorkLogResponseModel
    {
        public int PayPeriodId { get; set; }
        public int PaySlipId { get; set; }
        public int WorkLogId { get; set; }
        public string? TotalAmount { get; set; }
        public decimal? TotalActualWorkedHours { get; set; }
        public string? TotalEarnings { get; set; }
        public decimal? HourWorkInDay { get; set; }
    }
}
