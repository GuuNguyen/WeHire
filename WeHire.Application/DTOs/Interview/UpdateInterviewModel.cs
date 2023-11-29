using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Interview
{
    public class UpdateInterviewModel
    {
        public int InterviewId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
