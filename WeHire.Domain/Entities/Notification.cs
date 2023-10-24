using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsRead { get; set; }
        public int? SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int? NotiTypeId { get; set; }

        public virtual NotificationType NotiType { get; set; }
        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
    }
}
