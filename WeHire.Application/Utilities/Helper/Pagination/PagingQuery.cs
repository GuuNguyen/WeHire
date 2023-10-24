using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.Helper.Pagination
{
    public class PagingQuery
    {
        [DefaultValue(PagingConstant.DEFAULT_PAGE)]
        public int PageIndex { get; set; }

        [DefaultValue(PagingConstant.DEFAULT_PAGE_SIZE)]
        public int PageSize { get; set; }
    }
}
