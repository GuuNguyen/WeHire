using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Interview
    {
        public Interview()
        {
            DeveloperInterviews = new HashSet<DeveloperInterview>();
        }

        public int InterviewId { get; set; }
        public string InterviewCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? NumOfInterviewee { get; set; }
        public string MeetingLink { get; set; }
        public string OutlookLink { get; set; }
        public string RejectionReason { get; set; }
        public DateTime? CreateAt { get; set; }
        public int Status { get; set; }
        public int? AssignStaffId { get; set; }
        public int? RequestId { get; set; }

        public virtual User AssignStaff { get; set; }
        public virtual HiringRequest Request { get; set; }
        public virtual ICollection<DeveloperInterview> DeveloperInterviews { get; set; }
    }
}
