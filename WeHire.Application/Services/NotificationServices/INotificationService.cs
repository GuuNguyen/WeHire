using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Notification;

namespace WeHire.Application.Services.NotificationServices
{
    public interface INotificationService
    {
        public Task<int> GetNotificationCount(int userId);
        public Task<List<GetNotiDetail>> GetNotificationByManagerAsync();
        public Task<List<GetNotiDetail>> GetNotificationAsync(int userId);
        public Task ReadNotification(int notificationId, int userId);
        public Task UnNewNotification(int userId);
        public Task SendManagerNotificationAsync(string SenderName, int routeId, string notiType, string content);
        public Task SendNotificationAsync(int? receiverId, int routeId, string notiType, string content);
        public Task<object> TestSendNotificationFirebase(string deviceToken, string title, string content,
                                                         string notificationType, int routeId);
    }
}
