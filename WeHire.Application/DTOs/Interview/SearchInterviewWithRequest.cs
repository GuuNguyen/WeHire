using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Interview
{
    public class SearchInterviewWithRequest
    {
        public string? Title { get; set; }
        public int? RequestId { get; set; }
        public DateTime? DateOfInterview { get; set; }
        public int? Status { get; set; }
    }
}
