using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Interview
{
    public class GetListInterview
    {
        public int InterviewId { get; set; }
        public string InterviewCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateOfInterview { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? NumOfInterviewee { get; set; }
        public string MeetingLink { get; set; }
        public string OutlookLink { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }
    }
}
