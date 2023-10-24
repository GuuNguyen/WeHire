using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class NotificationType
    {
        public NotificationType()
        {
            Notifications = new HashSet<Notification>();
        }

        public int NotiTypeId { get; set; }
        public string NotiTypeName { get; set; }
        public int Status { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
