using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Notification;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Entity.Repositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Infrastructure.Services.NotificationServices
{
    internal class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetNotiDetail>> GetNotificationAsync(int userId)
        {
            var notificationsQuery = _unitOfWork.UserNotificationRepository
                                            .Get(un => un.UserId == userId)
                                            .AsNoTracking()
                                            .Include(un => un.Notification.NotiType)
                                            .Include(un => un.Notification.UserNotifications)
                                            .Select(un => un.Notification);

            var notifications = await notificationsQuery
                                .OrderByDescending(n => n.CreatedAt)
                                .ToListAsync();

            var distinctNotifications = notifications
                                .GroupBy(n => n.NotificationId)
                                .Select(group => group.First())
                                .ToList();

            var mappedNotis = _mapper.Map<List<GetNotiDetail>>(distinctNotifications);

            return mappedNotis;
        }

        public async Task<List<GetNotiDetail>> GetNotificationByManagerAsync()
        {
            var notificationsQuery = _unitOfWork.UserNotificationRepository
                                            .Get(un => un.User.RoleId == (int)RoleEnum.Manager)
                                            .AsNoTracking()
                                            .Include(un => un.User)
                                            .Include(un => un.User.CompanyPartners)
                                            .Include(un => un.Notification.NotiType)
                                            .Include(un => un.Notification.UserNotifications)
                                            .Select(un => un.Notification);

            var notifications = await notificationsQuery
                                .OrderByDescending(n => n.CreatedAt)
                                .ToListAsync();

            var distinctNotifications = notifications
                                .GroupBy(n => n.NotificationId)
                                .Select(group => group.First())
                                .ToList();

            var mappedNotis = _mapper.Map<List<GetNotiDetail>>(distinctNotifications);
            return mappedNotis;
        }

        public async Task SendNotificationAsync(int? receiverId, int routeId, string notiType, string content)
        {
            var type = _unitOfWork.NotificationTypeRepository.Get(nt => nt.NotiTypeName.Equals(notiType)).SingleOrDefault();
            var newNoti = new Domain.Entities.Notification
            {
                SenderName = "WeHire",
                Content = content,
                CreatedAt = DateTime.Now,
                NotiTypeId = type.NotiTypeId,
                RouteId = routeId,
                UserNotifications = new List<UserNotification>()
                {
                    new UserNotification
                    {
                        UserId = (int)receiverId!,
                        IsRead = false,
                        IsNew = true,
                    }
                }
            };
            await _unitOfWork.NotificationRepository.InsertAsync(newNoti);
            await _unitOfWork.SaveChangesAsync();

            await SendMessageToDevice(new List<int> { (int)receiverId }, notiType, content, notiType, routeId);
        }

        public async Task ReadNotification(int notificationId, int userId)
        {
            var noti = await _unitOfWork.UserNotificationRepository.Get(n => n.NotificationId == notificationId &&
                                                                             n.UserId == userId)
                                                                   .SingleOrDefaultAsync();
            noti.IsRead = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnNewNotification(int userId)
        {
            var noti = await _unitOfWork.UserNotificationRepository.Get(n => n.UserId == userId)
                                                                   .ToListAsync();
            foreach (var notification in noti)
            {
                notification.IsNew = false;
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendManagerNotificationAsync(string SenderName, int routeId, string notiType, string content)
        {
            var type = _unitOfWork.NotificationTypeRepository.Get(nt => nt.NotiTypeName.Equals(notiType)).SingleOrDefault();
            var newNoti = new Domain.Entities.Notification
            {
                SenderName = SenderName,
                Content = content,
                CreatedAt = DateTime.Now,
                NotiTypeId = type.NotiTypeId,
                RouteId = routeId
            };
            await _unitOfWork.NotificationRepository.InsertAsync(newNoti);
            await _unitOfWork.SaveChangesAsync();

            var managers = _unitOfWork.UserRepository.Get(m => m.RoleId == (int)RoleEnum.Manager &&
                                                      m.Status == (int)UserStatus.Active)
                                                     .ToList();

            newNoti.UserNotifications = managers.Select(m =>
                                            new UserNotification
                                            {
                                                Notification = newNoti,
                                                User = m,
                                                IsRead = false,
                                                IsNew = true,
                                            }).ToList();
            var managerIds = managers.Select(m => m.UserId).ToList();

            await SendMessageToDevice(managerIds, notiType, content, notiType, routeId);
        }

        private async Task SendMessageToDevice(IEnumerable<int> userIds, string title, string content, string? NotiType = null, int? routeId = null)
        {
            var deviceTokens = await _unitOfWork.UserDeviceRepository.Get(u => userIds.ToList().Contains((int)u.UserId!))
                                                                     .Select(u => u.DeviceToken).ToListAsync();
            if(deviceTokens.Any())
            {
                foreach(var deviceToken in deviceTokens)
                {
                    await SendFirebaseMessageAsync(deviceToken, title, content, NotiType, routeId);
                }
            }       
        }

        private async Task SendFirebaseMessageAsync(string deviceToken, string title, string content, 
                                                    string? notificationType = null, int? routeId = null)
        {
            try
            {
                var message = new FirebaseAdmin.Messaging.Message
                {
                    Token = deviceToken,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = content
                    },
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification
                        {
                            Color = "#09ff00",
                            Sound = "default"
                        }
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "notificationType", notificationType ?? ""},
                        { "routeId", routeId.ToString() ?? ""}
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine("Successfully sent message: " + response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<int> GetNotificationCount(int userId)
        {
            var count = await _unitOfWork.UserNotificationRepository.Get(un => un.UserId ==  userId &&
                                                                               un.IsNew == true)
                                                                    .CountAsync();
            return count;
        }

        public async Task<object> TestSendNotificationFirebase(string deviceToken, string title, string content, string notificationType, int routeId)
        {
            try
            {
                var message = new FirebaseAdmin.Messaging.Message
                {
                    Token = deviceToken,
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = title,
                        Body = content
                    },
                    Android = new AndroidConfig()
                    {
                        Notification = new AndroidNotification
                        {
                            Color = "#09ff00",
                            Sound = "default"
                        }
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "notificationType", notificationType ?? ""},
                        { "routeId", routeId.ToString() ?? ""}
                    }
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
      
    }
}
