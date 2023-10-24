using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Type
{
    public class GetTypeDetail
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public string TypeDescription { get; set; }
        public string StatusString { get; set; }
    }
}
