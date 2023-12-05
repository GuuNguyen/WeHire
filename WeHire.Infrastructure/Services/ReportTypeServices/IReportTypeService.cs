using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ReportType;

namespace WeHire.Infrastructure.Services.ReportTypeServices
{
    public interface IReportTypeService
    {
        public List<GetReportTypeModel> GetReportTypes();
    }
}
