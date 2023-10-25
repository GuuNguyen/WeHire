using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Notification;

namespace WeHire.Infrastructure.Services.NotificationServices
{
    public interface INotificationService
    {
       public Task<List<GetNotiDetail>> GetNotificationByManagerAsync();
       public Task<List<GetNotiDetail>> GetNotificationByHRAsync(int hrId);
       public Task SendManagerNotificationAsync(int senderId, string notiTypeString);
       public Task SendHRNotificationAsync(int receiverId, string notiTypeString);
    }
}
