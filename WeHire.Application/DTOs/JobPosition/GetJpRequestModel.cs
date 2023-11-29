using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;

namespace WeHire.Application.DTOs.JobPosition
{
    public class GetJpRequestModel
    {
        public int JobPositionId { get; set; }
        public string PositionName { get; set; }
        public int TotalHiringRequest { get; set; }
        public List<GetRequestInJobPosition> RequestsInJobPosition { get; set; }
    }
}
