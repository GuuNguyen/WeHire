using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Notification;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, GetNotiDetail>()
                 .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotiType.NotiTypeName))
                 .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.SenderName))
                 .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
                 .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.UserNotifications.FirstOrDefault()!.IsRead))
                 .ForMember(dest => dest.IsNew, opt => opt.MapFrom(src => src.UserNotifications.FirstOrDefault()!.IsNew))
                 .ReverseMap();
        }
    }
}
