using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class PayPeriodInMonth
    {
        public string MonthYearMMMM { get; set; }
        public string InputDate { get; set; }
        public string StartDateMMM { get; set; }
        public string StartDate { get; set; }
        public string EndDateMMM { get; set; }
        public string EndDate { get; set; }
    }
}
