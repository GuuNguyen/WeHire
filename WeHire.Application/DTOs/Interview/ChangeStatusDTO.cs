using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Interview
{
    public class ChangeStatusDTO
    {
        public int InterviewId { get; set; }
        public bool isApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
