using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.SelectingDev
{
    public class ChangeStatusToInterviewingDTO
    {
        public int RequestId { get; set; }
        public int InterviewId { get; set; }
        public List<int> DevIds { get; set; }
    }
}
