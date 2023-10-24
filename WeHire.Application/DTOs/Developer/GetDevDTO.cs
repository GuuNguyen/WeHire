using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Developer
{
    public class GetDevDTO
    {
        public int DeveloperId { get; set; }
        public int? UserId { get; set; }
        public string CodeName { get; set; }
        public int? YearOfExperience { get; set; }
        public int? AverageSalary { get; set; }
        public int? ScheduleTypeId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? LevelId { get; set; }
        public string? DevStatusString { get; set; }
        public int? Cvid { get; set; }
    }
}
