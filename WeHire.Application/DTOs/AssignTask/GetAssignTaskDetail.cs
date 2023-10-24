using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class GetAssignTaskDetail
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string StatusString { get; set; }
        public int? UserId { get; set; }
        public string StaffName { get; set; }
    }
}
