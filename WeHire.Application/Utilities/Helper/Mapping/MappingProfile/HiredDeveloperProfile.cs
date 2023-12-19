using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiredDeveloper;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.ContractEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class HiredDeveloperProfile : Profile
    {
        public HiredDeveloperProfile()
        {
            CreateMap<HiredDeveloper, GetHiredDeveloperModel>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiredDeveloperStatus)src.Status)))
                .ReverseMap();

            CreateMap<HiredDeveloper, GetDeveloperInProject>()
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Developer.UserId))
               .ForMember(dest => dest.CodeName, opt => opt.MapFrom(src => src.Developer.CodeName))
               .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.Developer.User.UserImage))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Developer.User.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Developer.User.LastName))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Developer.User.PhoneNumber))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Developer.User.Email))
               .ForMember(dest => dest.YearOfExperience, opt => opt.MapFrom(src => src.Developer.YearOfExperience))
               .ForMember(dest => dest.AverageSalary, opt => opt.MapFrom(src => src.Developer.AverageSalary))
               .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.Developer.EmploymentType.EmploymentTypeName))
               .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.Developer.Level.LevelName))
               .ForMember(dest => dest.GenderString, opt => opt.MapFrom(src => src.Developer.Gender.GenderName))
               .ForMember(dest => dest.TypeRequireStrings, opt => opt.MapFrom(src => src.Developer.DeveloperTypes.Select(dt => dt.Type.TypeName)))
               .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.Developer.DeveloperSkills.Select(dt => dt.Skill.SkillName)))
               .ForMember(dest => dest.HiredDevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiredDeveloperStatus)src.Status)))
               .ForMember(dest => dest.StartWorkingDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.Contract.FromDate)))
               .ForMember(dest => dest.EndWorkingDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.Contract.ToDate)));
        }
    }
}
