using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.HiringRequest
{
    public class SearchHiringRequestDTO
    {
        public string? JobTitle { get; set; } 
        public int? NumberOfDev { get; set; } 
        public int? TargetedDev { get; set; } 
        public DateTime? Duration { get; set; } 
        public int? TypeRequireId { get; set; } 
        public int? LevelRequireId { get; set; }
        public int? Status { get; set; }
    }
}
