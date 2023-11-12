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
            WaitingDevApproval = 1,

            [Description("Approved")]
            Approved = 2,

            [Description("Rejected")]
            Rejected = 3,
        }
    }
}
