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
            [Description("Preparing")]
            Preparing = 1,

            [Description("In process")]
            InProcess = 2,

            [Description("Closed")]
            Closed = 3,
        }
    }
}
