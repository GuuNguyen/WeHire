using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Project
{
    public class GetProjectDetail
    {
        public int ProjectId { get; set; }
        public string CompanyName { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public int? NumberOfDev { get; set; }
        public string BackgroundImage { get; set; }
        public string Description { get; set; }
        public string? MinStartDate { get; set; }
        public string? MaxEndDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartDateMMM { get; set; }
        public string EndDateMMM { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PostedTime { get; set; }
        public int DayLeft { get; set; }
        public double DayLeftPercent { get; set; }
        public string StatusString { get; set; }
    }
}
