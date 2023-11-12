using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.ProjectType
{
    public class GetProjectTypeDTO
    {
        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }
        public string StatusString { get; set; }
    }
}
