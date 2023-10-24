using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class ChangeStatusTaskDTO
    {
        public int TaskId { get; set; }
        public int Status { get; set; }
    }
}
