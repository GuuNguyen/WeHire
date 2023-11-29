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
            return inputTime == null ? "00:00" : inputTime!.Value.ToString(@"hh\:mm");
        }

        public static double ParseDurationToHours(string duration)
        {
            double totalHours = 0;

            string[] parts = duration.Split(new char[] { 'h', 'm' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (part.EndsWith("h"))
                {
                    double hours = double.Parse(part.TrimEnd('h'));
                    totalHours += hours;
                }
                else if (part.EndsWith("m"))
                {
                    double minutes = double.Parse(part.TrimEnd('m'));
                    totalHours += minutes / 60; 
                }
            }

            return totalHours;
        }

        public static decimal CalculateTotalWorkTime(TimeSpan? timeIn, TimeSpan? timeOut)
        {
            if(timeIn == null && timeOut == null) return 0;

            decimal totalWorkTime;
            var lunchTime = 0;

            if (timeIn < new TimeSpan(8, 0, 0))
            {
                timeIn = new TimeSpan(8, 0, 0);
            }
            if (timeOut >= new TimeSpan(13, 0, 0))
            {
                lunchTime = 60;
                if(timeOut > new TimeSpan(17, 0, 0))
                {
                    timeOut = new TimeSpan(17, 0, 0);
                }
            }

            var workedHour = timeOut.Value - timeIn.Value;

            totalWorkTime = (decimal)(workedHour.TotalHours - (lunchTime / 60));

            return totalWorkTime;
        }
    }
}
