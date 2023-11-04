using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.ConvertDate
{
    public static class ConvertTime
    {
        public static string ConvertTimeToShortFormat(TimeSpan? inputTime)
        {
            string shortFormat = inputTime!.Value.ToString(@"hh\:mm");
            return shortFormat;
        }
    }
}
