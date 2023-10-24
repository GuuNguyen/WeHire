using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ScheduleType;

namespace WeHire.Infrastructure.Services.ScheduleTypeServices
{
    public interface IScheduleTypeService
    {
        public List<GetScheduleTypeDTO> GetAllScheduleType();
    }
}
