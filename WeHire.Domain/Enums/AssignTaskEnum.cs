using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class AssignTaskEnum
    {
        public enum AssignTaskStatus
        {
            [Description("Preparing")]
            Preparing = 1,

            [Description("In Progress")]
            InProgress = 2,

            [Description("Done")]
            Done = 3,

            [Description("Cancelled")]
            Cancelled = 4,
        }
    }
}
