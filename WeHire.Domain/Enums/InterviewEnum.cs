using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class InterviewEnum
    {
        public enum InterviewStatus
        {
            [Description("Waiting Approval")]
            WaitingManagerApproval = 1,

            [Description("Interviewing")]
            Interviewing = 2,

            [Description("Manager Rejected")]
            ManagerRejected = 3,
        }
    }
}
