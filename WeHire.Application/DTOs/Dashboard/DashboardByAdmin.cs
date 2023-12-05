using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Dashboard
{
    public class DashboardByAdmin
    {
        public AccountDashboard AccountDashboard { get; set; }
        public string TotalMoney { get; set; }
        public int TotalProject { get; set; }
        public int TotalHiringRequest { get; set; }
    }
}
