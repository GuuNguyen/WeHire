using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.EmploymentType
{
    public class GetEmploymentTypeDTO
    {
        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }
        public string StatusString { get; set; }
    }
}
