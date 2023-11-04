using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class GetAssignTaskDTO
    {
        public int TaskId { get; set; }
        public int? UserId { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public string Deadline { get; set; }
        public int NumberOfInterviewee { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }    
    }
}
