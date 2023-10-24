using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Education
{
    public class GetEducationDTO
    {
        public int EducationId { get; set; }
        public int? DeveloperId { get; set; }
        public string MajorName { get; set; }
        public string SchoolName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}
