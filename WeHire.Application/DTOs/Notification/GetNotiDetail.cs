using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Notification
{
    public class GetNotiDetail
    {
        public int NotificationId { get; set; }
        public string NotificationTypeName { get; set; }
        public int RouteId { get; set; }
        public string CompanyName { get; set; }
        public string Content { get; set; }
        public string CreatedTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsNew { get; set; }
    }
}
