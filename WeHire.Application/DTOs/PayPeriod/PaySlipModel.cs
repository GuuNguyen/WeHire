using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class PaySlipModel
    {
        public string? Fullname { get; set; }
        public string? CodeName { get; set; }
        public string? Position { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<WorkLogModel> WorkLogs { get; set; }
    }
}
