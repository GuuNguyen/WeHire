using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.EnumDescription
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T value) where T : Enum
        {
            DescriptionAttribute descriptionAttribute = value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? string.Empty;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                DescriptionAttribute attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return value.ToString(); 
        }

    }
}
