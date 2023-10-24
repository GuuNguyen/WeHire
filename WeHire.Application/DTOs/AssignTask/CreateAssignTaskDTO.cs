using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class CreateAssignTaskDTO
    {
        public int? UserId { get; set; }
        public List<int> DevIds { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }        
    }
}
