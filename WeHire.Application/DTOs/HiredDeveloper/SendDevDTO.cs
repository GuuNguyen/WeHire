using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.HiredDeveloper
{
    public class SendDevDTO
    {
        [Required(ErrorMessage = "RequestId is required")]
        public int RequestId { get; set; }
        [Required(ErrorMessage = "DeveloperIds is required")]
        public List<int> DeveloperIds { get; set; }
    }
}
