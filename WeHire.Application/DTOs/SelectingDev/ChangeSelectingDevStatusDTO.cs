using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.SelectingDev
{
    public class ChangeSelectingDevStatusDTO
    {
        public int RequestId { get; set; }
        public int DeveloperId { get; set; }
        public bool isApproved { get; set; }
    }
}
