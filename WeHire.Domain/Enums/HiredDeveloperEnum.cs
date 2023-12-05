using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class HiredDeveloperEnum
    {
        public enum HiredDeveloperStatus
        {
            [Description("Under Consideration")]
            UnderConsideration = 1,

            [Description("Waiting Interview")]
            WaitingInterview = 2,

            [Description("Interview Scheduled")]
            InterviewScheduled = 3,

            [Description("Rejected")]
            Rejected = 4,

            [Description("Contract Processing")]
            ContractProcessing = 5,

            [Description("Contract Failed")]
            ContractFailed = 6,

            [Description("Request Closed")]
            RequestClosed = 7,

            [Description("Working")]
            Working = 8,

            [Description("Terminated")]
            Terminated = 9,

            [Description("Completed")]
            Completed = 10,
        }
    }
}
