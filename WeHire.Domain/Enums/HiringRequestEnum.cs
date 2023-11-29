using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class HiringRequestEnum
    {
        public enum HiringRequestStatus
        {
            [Description("Saved")]
            Saved = 0,

            [Description("Waiting Approval")]
            WaitingApproval = 1,

            [Description("In Progress")]
            InProgress = 2,

            [Description("Rejected")]
            Rejected = 3,

            [Description("Expired")]
            Expired = 4,

            [Description("Cancelled")]
            Cancelled = 5,

            [Description("Closed")]
            Closed = 6,

            [Description("Completed")]
            Completed = 7,
        }
    }
}
