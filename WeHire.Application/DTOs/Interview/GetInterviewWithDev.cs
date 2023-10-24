using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;

namespace WeHire.Application.DTOs.Interview
{
    public class GetInterviewWithDev
    {
        public int InterviewId { get; set; }
        public int? RequestId { get; set; }
        public string Title { get; set; }
        public string? InterviewerName { get; set; }
        public string? AssignStaffName { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? StatusString { get; set; }

        public List<GetAllFieldDev> Developers { get; set; }
    }
}
