using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class PayPeriodEnum
    {
        public enum PayPeriodStatus
        {
            [Description("Created")]
            Created = 1,
            [Description("Paid")]
            Paid = 2,
            [Description("Expired")]
            Expired = 3,
        }
    }
}
