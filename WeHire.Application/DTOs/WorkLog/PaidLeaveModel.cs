using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.WorkLog
{
    public class PaidLeaveModel
    {
        public int WorkLogId { get; set; }
        public bool IsPaidLeave { get; set; }
    }
}
