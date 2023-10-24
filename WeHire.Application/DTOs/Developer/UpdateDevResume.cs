using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class UpdateDevResume
    {
        public int DeveloperId { get; set; }
        public int? YearOfExperience { get; set; }
        public decimal? AverageSalary { get; set; }

        public int? LevelId { get; set; }
        public List<int> Types { get; set; }
        public List<int> Skills { get; set; }
    }
}
