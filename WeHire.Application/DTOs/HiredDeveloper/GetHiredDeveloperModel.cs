using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiredDeveloper
{
    public class GetHiredDeveloperModel
    {
        public int HiredDeveloperId { get; set; }       
        public int? ProjectId { get; set; }
        public int? RequestId { get; set; }
        public int? ContractId { get; set; }
        public int? DeveloperId { get; set; }
        public string StatusString { get; set; }
    }
}
