using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProfessionalExperience;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.LevelEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ProfessionalExperienceProfile : Profile
    {
        public ProfessionalExperienceProfile()
        {
            CreateMap<ProfessionalExperience, GetPEByAdmin>()
                .ForMember(dest => dest.DeveloperFullName, opt => opt.MapFrom(src => $"{src.Developer.User.FirstName} {src.Developer.User.LastName}"))
                .ForMember(dest => dest.DeveloperCode, opt => opt.MapFrom(src => src.Developer.CodeName))
                .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                .ReverseMap();
            CreateMap<ProfessionalExperience, GetProfessionalExperience>().ReverseMap();
            CreateMap<ProfessionalExperience, CreateProfessionalExperience>().ReverseMap();
            CreateMap<ProfessionalExperience, UpdateProfessionalExperience>().ReverseMap();
        }
    }
}
