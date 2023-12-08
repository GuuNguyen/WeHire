using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Interview
{
    public class CreateInterviewDTO
    {
        [Required(ErrorMessage = "RequestId is required")]
        public int RequestId { get; set; }
        [Required(ErrorMessage = "DeveloperId is required")]
        public int DeveloperId { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "DateOfInterview is required")]
        public DateTime DateOfInterview { get; set; }
        [Required(ErrorMessage = "StartTime is required")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "EndTime is required")]
        public TimeSpan EndTime { get; set; }
    }
}
