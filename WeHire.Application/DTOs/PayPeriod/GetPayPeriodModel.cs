using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class GetPayPeriodModel
    {
        public int PayPeriodId { get; set; }
        public int? ProjectId { get; set; }
        public string PayPeriodCode { get; set; }
        public string StartDateMMM { get; set; }
        public string EndDateMMM { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusString { get; set; }
    }
}
