using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.TeamMeeting
{
    public class OnlineMeetingModel
    {
        public int InterviewId { get; set; }
        public string RedirectUrl { get; set; }
        public string authenCode { get; set; }
    }
}
