using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.DeveloperTaskAssignment
{
    public class ChangeStatusDevTaskAssignmentDTO
    {
        public int DeveloperId { get; set; }
        public int TaskId { get; set; }
        public int Status { get; set; }
    }
}
