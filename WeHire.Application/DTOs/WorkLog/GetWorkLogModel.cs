using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.WorkLog
{
    public class GetWorkLogModel
    {
        public int WorkLogId { get; set; }
        public int? PaySlipId { get; set; }
        public string WorkDateMMM { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public decimal HourWorkInDay { get; set; }
    }
}
