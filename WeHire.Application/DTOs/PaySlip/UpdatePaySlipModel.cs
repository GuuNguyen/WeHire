using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.PaySlip
{
    public class UpdatePaySlipModel
    {
        [Required(ErrorMessage = "PaySlipId is required")]
        public int PaySlipId { get; set; }
        public decimal? TotalOvertimeHours { get; set; }
    }
}
