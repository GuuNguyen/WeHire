using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Infrastructure.Services.NotificationServices
{
    public interface INotificationService
    {
       public Task SendNotificationToManager(int receiverId);
    }
}
