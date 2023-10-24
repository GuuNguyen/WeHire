using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class AgreementEnum
    {
        public enum AgreementStatus
        {
            [Description("Pending Approval")]
            PendingApproval = 1,

            [Description("Pending Payment")]
            PendingPayment = 2,

            [Description("Paid")]
            Paid = 3,

            [Description("Terminated")]
            Terminated = 4,
            
            [Description("Expired")]
            Expired = 5,
        }
    }
}
