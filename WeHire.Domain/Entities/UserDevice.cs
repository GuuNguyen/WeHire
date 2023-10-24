using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class UserDevice
    {
        public int UserDeviceId { get; set; }
        public string DeviceToken { get; set; }
        public int? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
