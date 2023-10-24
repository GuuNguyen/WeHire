using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Domain.Enums
{
    public class TransactionEnum
    {
        public enum TransactionStatus
        {
            Created = 1,
            Success = 2,
            Failed = 3,
        }
    }
}
