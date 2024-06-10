using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Education;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class EducationProfile : Profile
    {
        public EducationProfile()
        {
            CreateMap<Education, GetEducationDTO>()
                .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                .ReverseMap();
            CreateMap<Education, GetEducationByAdmin>()
                .ForMember(dest => dest.DeveloperFullName, opt => opt.MapFrom(src => $"{src.Developer.User.FirstName} {src.Developer.User.LastName}"))
                .ForMember(dest => dest.DeveloperCode, opt => opt.MapFrom(src => src.Developer.CodeName))
                .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                .ReverseMap();
            CreateMap<Education, CreateEducationDTO>().ReverseMap();
            CreateMap<Education, UpdateEducationModel>().ReverseMap();
        }
    }
}
