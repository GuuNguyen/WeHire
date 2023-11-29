using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.JobPosition
{
    public class GetJobPosition
    {
        public int JobPositionId { get; set; }
        public int? ProjectId { get; set; }
        public string PositionName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusString { get; set; }       
    }
}
