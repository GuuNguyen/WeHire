using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.ConvertDate
{
    public static class ConvertDateTime
    {
        public static string ConvertDateToString(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("dd-MM-yyyy") : "";
        }
        public static DateTime ConvertTimeToSEA(DateTime date)
        {
            var seaDateTime = TimeZoneInfo.ConvertTime(date,
                          TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            return seaDateTime;
        }
    }
}
