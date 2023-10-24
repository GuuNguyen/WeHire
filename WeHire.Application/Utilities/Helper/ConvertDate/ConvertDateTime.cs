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
    }
}
