using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class CreateSelectedDev
    {
        public int RequestId { get; set; }
        public List<int> DeveloperIds { get; set; }
    }
}
