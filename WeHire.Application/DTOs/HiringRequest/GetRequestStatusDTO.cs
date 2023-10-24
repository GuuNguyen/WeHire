using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class GetRequestStatusDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
