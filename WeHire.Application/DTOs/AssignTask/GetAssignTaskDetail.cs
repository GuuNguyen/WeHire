using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.User;

namespace WeHire.Application.DTOs.AssignTask
{
    public class GetAssignTaskDetail
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public GetUserDetail Staff { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string StatusString { get; set; }
        public List<GetDevDTO> Devs { get; set; }
    }
}
