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
            [Description("Contract Processing")]
            ContractProcessing = 1,

            [Description("Working")]
            Working = 2,

            [Description("Terminated")]
            Terminated = 3,

            [Description("Completed")]
            Completed = 4,

            [Description("Contract Fail")]
            ContractFailed = 5,
        }
    }
}
