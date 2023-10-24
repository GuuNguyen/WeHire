using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.ScheduleType
{
    public class GetScheduleTypeDTO
    {
        public int ScheduleTypeId { get; set; }
        public string ScheduleTypeName { get; set; }
        public string StatusString { get; set; }
    }
}
