using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.HiringRequestEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class HiringRequestProfile : Profile
    {
        public HiringRequestProfile()
        {
            CreateMap<HiringRequest, GetRequestDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiringRequestStatus)src.Status)));

            CreateMap<HiringRequest, GetListHiringRequest>()
               .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Company.CompanyImage))
               .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertDate.ConvertDateTime.ConvertDateToString(src.Duration)))
               .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
               .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiringRequestStatus)src.Status)));
            
            CreateMap<HiringRequest, GetAllFieldRequest>()
                .ForMember(dest => dest.DurationMMM, opt => opt.MapFrom(src => ConvertDate.ConvertDateTime.ConvertDateToString(src.Duration)))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => ConvertDate.ConvertDateTime.ConvertDateToStringNumber(src.Duration)))
                .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiringRequestStatus)src.Status)))
                .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
                .ForMember(dest => dest.TypeRequireName, opt => opt.MapFrom(src => src.TypeRequire.TypeName))
                .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.LevelRequire.LevelName))
                .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.SkillRequires.Select(s => s.Skill.SkillName)));

            CreateMap<HiringRequest, GetRequestDetail>().ReverseMap();
            CreateMap<HiringRequest, CreateRequestDTO>().ReverseMap();
            CreateMap<HiringRequest, UpdateRequestDTO>().ReverseMap();
        }
    }
}
