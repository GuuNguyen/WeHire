using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class ContractEnum
    {
        public enum ContractStatus
        {
            [Description("Pending")]
            Pending = 1,
            [Description("Signed")]
            Signed = 2,
            [Description("Failed")]
            Failed = 3
        }
    }
}
