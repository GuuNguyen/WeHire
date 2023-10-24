using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.DeveloperTaskAssignment
{
    public class GetDevTaskAssignmentDTO
    {
        public int DeveloperId { get; set; }
        public int TaskId { get; set; }
        public string StatusString { get; set; }
    }
}
