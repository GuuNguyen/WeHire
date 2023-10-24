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
            [Description("Waiting Dev Approval")]
            WaitingDevAccept = 1,

            [Description("Dev Accepted")]
            DevAccepted = 2,

            [Description("Dev Rejected")]
            DevRejected = 3,

            [Description("Waiting HR Approval")]
            WaitingHRAccept = 4,

            [Description("Waiting Interview")]
            WaitingInterview = 5,

            [Description("HR Rejected")]
            HRRejected = 6,

            [Description("Interviewing")]
            Interviewing = 7,

            [Description("Onboarding")]
            OnBoarding = 8,
        }
    }
}
