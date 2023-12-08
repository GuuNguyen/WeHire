using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Report
{
    public class ReplyReport
    {
        [Required(ErrorMessage = "ReportId is required")]
        public int ReportId { get; set; }
        [Required(ErrorMessage = "ResponseContent is required")]
        public string ResponseContent { get; set; }
    }
}
