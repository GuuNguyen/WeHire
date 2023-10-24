using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Interview
{
    public class SearchInterviewDTO
    {
        public string? Title { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public int? Status { get; set; }
    }
}
