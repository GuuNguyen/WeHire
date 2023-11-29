using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PaySlip
{
    public class UpdatePaySlipModel
    {
        public int PaySlipId { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
    }
}
