using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class PaySlip
    {
        public PaySlip()
        {
            WorkLogs = new HashSet<WorkLog>();
        }

        public int PaySlipId { get; set; }
        public decimal? TotalActualWorkedHours { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public decimal? TotalEarnings { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? PayPeriodId { get; set; }
        public int? HiredDeveloperId { get; set; }

        public virtual HiredDeveloper HiredDeveloper { get; set; }
        public virtual PayPeriod PayPeriod { get; set; }
        public virtual ICollection<WorkLog> WorkLogs { get; set; }
    }
}
