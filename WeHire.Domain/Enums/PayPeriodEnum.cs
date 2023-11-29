using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class PayPeriodEnum
    {
        public enum PayPeriodStatus
        {
            Created = 1,
            Paid = 2,
            Expired = 3,
            Deleted = 4,
        }
    }
}
