using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PayPeriod;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.LevelEnum;
using static WeHire.Domain.Enums.PayPeriodEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class PayPeriodProfile :Profile
    {
        public PayPeriodProfile()
        {
            CreateMap<PayPeriod, GetPayPeriodModel>()
                   .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                   .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                   .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(PayPeriodStatus), src.Status)))
                   .ReverseMap();
            CreateMap<PayPeriod, GetPayPeriodBill>()
                   .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Project.Company.CompanyName))
                   .ForMember(dest => dest.CompanyAddress, opt => opt.MapFrom(src => src.Project.Company.Address))
                   .ForMember(dest => dest.CompanyEmail, opt => opt.MapFrom(src => src.Project.Company.CompanyEmail))
                   .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Project.Company.CompanyImage))
                   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
                   .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.UpdatedAt)))
                   .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                   .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                   .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(PayPeriodStatus), src.Status)))
                   .ReverseMap();
            CreateMap<PayPeriod, CreatePayPeriodModel>().ReverseMap();
        }
    }
}
