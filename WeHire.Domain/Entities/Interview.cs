using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Interview
    {
        public int InterviewId { get; set; }
        public string InterviewCode { get; set; }
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? NumOfInterviewee { get; set; }
        public string MeetingUrl { get; set; }
        public string OutlookUrl { get; set; }
        public string RejectionReason { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Status { get; set; }
        public int? DeveloperId { get; set; }
        public int? RequestId { get; set; }

        public virtual Developer Developer { get; set; }
        public virtual HiringRequest Request { get; set; }
    }
}
