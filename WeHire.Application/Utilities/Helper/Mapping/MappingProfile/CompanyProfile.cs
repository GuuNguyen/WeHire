using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.CompanyPartnerEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyPartner, GetCompanyDetail>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(CompanyStatus), src.Status)))
                .ForMember(dest => dest.HRFullName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ReverseMap();
            CreateMap<CompanyPartner, GetCompanyDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(CompanyStatus), src.Status)))
                .ReverseMap();
            CreateMap<CompanyPartner, CreateCompanyDTO>().ReverseMap();
            CreateMap<CompanyPartner, UpdateCompanyDTO>().ReverseMap();
        }
    }
}
