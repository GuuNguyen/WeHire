using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.CheckNullProperties
{
    public static class CheckNullHelper
    {
        public static bool AreAllPropertiesNull<T>(this T obj)
        {
            if (obj == null)
            {
                return true;
            }

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);

                if (value != null)
                {
                    if (property.PropertyType.IsValueType)
                    {
                        return false;
                    }

                    if (property.PropertyType == typeof(string) && !string.IsNullOrEmpty((string)value))
                    {
                        return false;
                    }

                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var listValue = value as IList;
                        if (listValue != null && listValue.Count > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
