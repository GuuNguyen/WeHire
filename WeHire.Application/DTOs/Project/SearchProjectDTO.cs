using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Project
{
    public class SearchProjectDTO
    {
        public int? ProjectTypeId { get; set; }
        public int? Status { get; set; }
    }
}
