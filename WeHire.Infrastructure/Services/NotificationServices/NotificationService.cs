using AutoMapper;
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
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Infrastructure.Services.NotificationServices
{
    internal class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<List<GetNotiDetail>> GetNotificationByHRAsync(int hrId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetNotiDetail>> GetNotificationByManagerAsync()
        {
            var hrNotifications = await _unitOfWork.UserNotificationRepository.Get(un => un.User.RoleId == (int)RoleEnum.Manager).AsNoTracking()
                                                                              .Include(un => un.User)
                                                                              .Include(un => un.User.CompanyPartners)
                                                                              .Include(un => un.Notification.NotiType)
                                                                              .Include(un => un.Notification.UserNotifications)
                                                                              .Select(un => un.Notification)
                                                                              .ToListAsync();
            var mappedNotis = _mapper.Map<List<GetNotiDetail>>(hrNotifications);
            var senderIds = hrNotifications.Select(n => n.SenderId).ToList();

            var companyNames = _unitOfWork.CompanyRepository
                               .Get(c => senderIds.Contains(c.UserId))
                               .ToDictionary(c => c.UserId, c => c.CompanyName);

            foreach (var noti in mappedNotis)
            {
                if (companyNames.TryGetValue(noti.SenderId, out var companyName))
                {
                    noti.CompanyName = companyName;
                }
            }

            return mappedNotis;
        }

        public Task SendHRNotificationAsync(int receiverId, string notiTypeString)
        {
            throw new NotImplementedException();
        }

        public async Task SendManagerNotificationAsync(int senderId, string notiTypeString)
        {
            var notiType = await _unitOfWork.NotificationTypeRepository.GetFirstOrDefaultAsync(nt => nt.NotiTypeName.Equals(notiTypeString));
            var newNoti = new Notification
            {
                SenderId = senderId,
                Content = GetContent(notiTypeString),
                CreateAt = DateTime.Now,
                NotiTypeId = notiType.NotiTypeId,
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
        }

        private string GetContent(string notiTypeString)
        {
            switch (notiTypeString)
            {
                case NotificationTypeString.DEVELOPER_RECRUITMENT:
                    return "posted a request to hire developers for their company. The request is awaiting your approval.";
                case NotificationTypeString.PENDING_INTERVIEW_APPROVAL:
                    return "submitted an interview request for their request. The request is awaiting your approval.";
                case NotificationTypeString.PENDING_AGREEMENT_APPROVAL:
                    return "sent you a Commission Agreement. The request is awaiting your approval.";
                default:
                    return "Undefined";
            }
        }
    }
}
