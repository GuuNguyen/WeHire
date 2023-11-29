using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PaySlip
{
    public class GetPaySlipModel
    {
        public int PaySlipId { get; set; }
        public int? PayPeriodId { get; set; }
        public int? HiredDeveloperId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal? TotalActualWorkedHours { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public string TotalEarnings { get; set; }
        public DateTime? CreatedAt { get; set; }  
    }
}
