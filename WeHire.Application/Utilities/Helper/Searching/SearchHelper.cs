using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.Searching
{
    public static class SearchHelper
    {
        public static IQueryable<T> SearchItems<T>(this IQueryable<T> query, object searchModel)
        where T : class
        {
            if (searchModel == null)
            {
                return query;
            }

            foreach (var prop in searchModel.GetType().GetProperties())
            {
                var value = prop.GetValue(searchModel, null);
                if (value != null)
                {
                    var param = Expression.Parameter(typeof(T), "entity");
                    var entityProp = Expression.Property(param, prop.Name);

                    Expression body = null;

                    switch (value)
                    {
                        case string strValue when !string.IsNullOrEmpty(strValue):
                            body = Expression.Call(entityProp, "Contains", null, Expression.Constant(strValue));
                            break;

                        case int intValue:
                            if (entityProp.Type.IsNullableType())
                                body = Expression.Equal(entityProp, Expression.Constant(intValue, entityProp.Type));
                            else
                                body = Expression.Equal(entityProp, Expression.Constant(intValue));
                            break;
                        case DateTime dateValue:
                            body = Expression.Equal(entityProp, Expression.Constant(dateValue));
                            break;

                        case byte byteValue:
                            body = Expression.Equal(entityProp, Expression.Constant(byteValue));
                            break;

                        default:
                            // Handle other data types if needed.
                            break;
                    }

                    if (body != null)
                    {
                        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                        query = query.Where(lambda);
                    }
                }
            }
            return query;
        }

    }
}
