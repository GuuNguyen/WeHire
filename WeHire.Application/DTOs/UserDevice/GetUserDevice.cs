using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.UserDevice
{
    public class GetUserDevice
    {
        public int UserDeviceId { get; set; }
        public int? UserId { get; set; }
        public string DeviceToken { get; set; }
    }
}
