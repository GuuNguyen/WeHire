using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PaySlip;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.PayPeriodEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class PaySlipProfile : Profile
    {
        public PaySlipProfile()
        {
            CreateMap<PaySlip, GetPaySlipModel>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.HiredDeveloper.Developer.User.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.HiredDeveloper.Developer.User.LastName))
                 .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.HiredDeveloper.Developer.User.Email))
                 .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.TotalEarnings!.Value.ToString("#,##0 VND")))
                 .ReverseMap();
            CreateMap<PaySlip, GetPaySlipByDevModel>()
                 .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((PayPeriodStatus)src.PayPeriod.Status)))
                 .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.TotalEarnings.Value.ToString("#,##0 VND") ?? ""))
                 .ForMember(dest => dest.WorkForMonth, opt => opt.MapFrom(src => src.PayPeriod.EndDate.Value.ToString("MMMM yyyy") ?? ""))
                 .ReverseMap();
            CreateMap<PaySlip, UpdatePaySlipModel>().ReverseMap();
        }
    }
}
