using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class GetRequestInJobPosition
    {
        public int RequestId { get; set; }
        public string JobTitle { get; set; }
        public int NumberOfDev { get; set; }
        public int TargetedDev { get; set; }
        public string DurationMMM { get; set; }
        public string PostedTime { get; set; }
        public string StatusString { get; set; }
    }
}
