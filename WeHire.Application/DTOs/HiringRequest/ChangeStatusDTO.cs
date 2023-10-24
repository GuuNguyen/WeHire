using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class ChangeStatusDTO
    {
        public class WaitingStatus
        {
            public int RequestId { get; set; }
            public string RejectionReason { get; set; } = string.Empty;
            public bool isApproved { get; set; }
        }

        public class ExpiredStatus
        {
            public int RequestId { get; set;}
            public DateTime NewDuration { get; set;}
            public bool isExtended { get; set; }
        }
    }
}
