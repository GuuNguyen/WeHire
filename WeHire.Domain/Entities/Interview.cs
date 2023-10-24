using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Interview
    {
        public Interview()
        {
            SelectedDevs = new HashSet<SelectedDev>();
        }

        public int InterviewId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string MeetingLink { get; set; }
        public int Status { get; set; }
        public int? InterviewerId { get; set; }
        public int? AssignStaffId { get; set; }
        public int? RequestId { get; set; }

        public virtual User AssignStaff { get; set; }
        public virtual User Interviewer { get; set; }
        public virtual HiringRequest Request { get; set; }
        public virtual ICollection<SelectedDev> SelectedDevs { get; set; }
    }
}
