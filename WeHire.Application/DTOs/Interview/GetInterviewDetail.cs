﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;

namespace WeHire.Application.DTOs.Interview
{
    public class GetInterviewDetail
    {
        public int InterviewId { get; set; }
        public int? RequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? NumOfInterviewee { get; set; }
        public string MeetingLink { get; set; }
        public string OutlookLink { get; set; }
        public string RejectionReason { get; set; }
        public string DateOfInterview { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string PostedTime { get; set; }
        public string? StatusString { get; set; }
    }
}
