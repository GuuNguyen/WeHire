using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Contract
{
    public class GetPreContract
    {
        public string CompanyPartnerName { get; set; }
        public string CompanyPartPhoneNumber { get; set; }
        public string CompanyPartnerAddress { get; set; }
        public string LegalRepresentation { get; set; }
        public string LegalRepresentationPosition { get; set; }
        public string DeveloperName { get; set; }
        public string DeveloperPhoneNumber { get; set; }
        public int? YearOfExperience { get; set; }
        public decimal? BasicSalary { get; set; }
        public string EmploymentType { get; set; }
        public string ProjectTitle { get; set; }
        public int? StandardMonthlyWorkingHours { get; set; }
        public decimal? OvertimePayMultiplier { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
