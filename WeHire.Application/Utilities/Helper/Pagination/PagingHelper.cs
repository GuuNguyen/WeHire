using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.Pagination
{
    public static class PagingHelper
    {
        public static IEnumerable<T> PagedItems<T>(this IEnumerable<T> query,
                                                             int page,
                                                             int pageSize,
                                                             int PageSizeLimit = PagingConstant.MAX_PAGE_SIZE)
        where T : class
        {
            if (pageSize > PageSizeLimit)
                throw new Exception("Input page size is over limitation.");

            if (query == null)
                return Enumerable.Empty<T>();

            if (page <= 0 && pageSize <= 0)
                return query.ToList();

            query = query
                       .Skip(pageSize * (page - 1))
                       .Take(pageSize);
            return query;
        }
    }
}
