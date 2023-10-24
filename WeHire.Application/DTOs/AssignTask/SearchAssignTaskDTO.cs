using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class SearchAssignTaskDTO
    {
        public string? TaskTitle { get; set; }
        public DateTime? Deadline { get; set; }
        public int? Status { get; set; }
        public int? UserId { get; set; }
    }
}
