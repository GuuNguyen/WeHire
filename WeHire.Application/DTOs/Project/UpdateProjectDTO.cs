using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Project
{
    public class UpdateProjectDTO 
    {
        [Required(ErrorMessage = "ProjectId is required")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "ProjectTypeId is required")]
        public int? ProjectTypeId { get; set; }
        [Required(ErrorMessage = "ProjectName is required")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "StartDate is required")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required")]
        public DateTime? EndDate { get; set; }
    }
}
