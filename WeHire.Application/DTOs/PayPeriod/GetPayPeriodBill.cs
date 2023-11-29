using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class GetPayPeriodBill
    {
        public int PayPeriodId { get; set; }
        public int? ProjectId { get; set; }
        public string PayPeriodCode { get; set; }
        public string StartDateMMM { get; set; }
        public string EndDateMMM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string StatusString { get; set; }

        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyImage { get; set; }
        public string TotalOTAmount { get; set; }
        public string TotalActualAmount { get; set; }
        public string TotalAmount { get; set; }
        public List<string> DeveloperFullName { get; set; }
    }
}
