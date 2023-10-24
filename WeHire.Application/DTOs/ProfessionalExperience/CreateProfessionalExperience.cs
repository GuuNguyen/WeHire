using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.ProfessionalExperience
{
    public class CreateProfessionalExperience
    {
        public int DeveloperId { get; set; }
        public string JobName { get; set; }
        public string CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
       
    }
}
