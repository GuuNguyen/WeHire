using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class WorkLog
    {
        public int WorkLogId { get; set; }
        public DateTime? WorkDate { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public bool? IsPaidLeave { get; set; }
        public int? PaySlipId { get; set; }

        public virtual PaySlip PaySlip { get; set; }
    }
}
