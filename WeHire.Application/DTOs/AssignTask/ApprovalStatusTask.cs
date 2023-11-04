using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.AssignTask
{
    public class ApprovalStatusTask
    {
        public int TaskId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;

        public bool IsApproval { get; set; }
    }
}
