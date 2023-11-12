using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class ProjectEnum
    {
        public enum ProjectStatus
        {
            [Description("Saved")]
            Saved = 1,

            [Description("Preparing")]
            Preparing = 2,

            [Description("Hiring Process")]
            HiringProcess = 3,

            [Description("In Process")]
            InProcess = 4,

            [Description("Closed")]
            Closed = 5,
        }
    }
}
