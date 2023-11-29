using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.PaySlip;
using WeHire.Domain.Entities;

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
                 .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.TotalEarnings!.Value.ToString("#,##0 VND") ))
                 .ReverseMap();
        }
    }
}
