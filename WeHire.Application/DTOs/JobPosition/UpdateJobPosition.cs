using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.JobPosition
{
    public class UpdateJobPosition
    {
        public int JobPositionId { get; set; }
        public string PositionName { get; set; }
    }
}
