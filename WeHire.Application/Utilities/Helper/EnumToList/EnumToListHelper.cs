using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.EnumToList
{
    public static class EnumToListHelper
    {
        public static List<EnumDetailDTO> ConvertEnumToListValue<T>() where T : Enum 
        {
            var enumValues = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new EnumDetailDTO
                {
                    StatusId = Convert.ToInt32(e),
                    StatusName = e.ToString(),
                })
                .ToList();
            return enumValues;
        }
    }
    public class EnumDetailDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
    }
}
