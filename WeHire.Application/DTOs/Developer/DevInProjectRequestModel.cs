using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class DevInProjectRequestModel
    {
        public int ProjectId { get; set; }
        public List<int>? Status { get; set; }
    }
}
