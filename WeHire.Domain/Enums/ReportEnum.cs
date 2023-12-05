using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class ReportEnum
    {
        public enum ReportStatus
        {
            [Description("Pending")]
            Pending = 1,

            [Description("Processing")]
            Processing = 2,

            [Description("Done")]
            Done = 3,
        }
    }
}
