using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Report
{
    public class CreateReportModel
    {
        [Required(ErrorMessage = "DeveloperId is required")]
        public int? DeveloperId { get; set; }
        [Required(ErrorMessage = "ProjectId is required")]
        public int? ProjectId { get; set; }
        [Required(ErrorMessage = "ReportTypeId is required")]
        public int? ReportTypeId { get; set; }
        [Required(ErrorMessage = "ReportTitle is required")]
        public string ReportTitle { get; set; }
        [Required(ErrorMessage = "ReportContent is required")]
        public string ReportContent { get; set; }
    }
}
