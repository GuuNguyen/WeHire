using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.Project
{
    public class CreateProjectDTO : FileDTO
    {
        public int? CompanyId { get; set; }
        public string ProjectName { get; set; }
        public int? ProjectTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}
