using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class SearchDeveloperDTO
    {
        public int? YearOfExperience { get; set; } = null;
        public int? AverageSalary { get; set; } = null;
    }
}
