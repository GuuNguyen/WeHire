using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class SelectedDevEnum
    {
        public enum SelectedDevStatus
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

            [Description("Onboarding")]
            OnBoarding = 6,

            [Description("Contract Failed")]
            ContractFailed = 7,

            [Description("Request Closed")]
            RequestClosed = 8,
        }
    }   
}
