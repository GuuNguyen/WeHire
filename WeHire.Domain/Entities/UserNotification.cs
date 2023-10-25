using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class UserNotification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public bool? IsRead { get; set; }
        public bool? IsNew { get; set; }

        public virtual Notification Notification { get; set; }
        public virtual User User { get; set; }
    }
}
