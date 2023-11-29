using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.JobPosition
{
    public class CreateJobPosition
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public string PositionName { get; set; }
    }
}
