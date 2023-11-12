using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Project
{
    public class SearchProjectDTO
    {
        public string? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public int? ProjectTypeId { get; set; }
        public int? Status { get; set; }
    }
}
