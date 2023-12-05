using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class DeveloperProfile : Profile
    {
        public DeveloperProfile()
        {
            CreateMap<Developer, GetDevDetail>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.User.Password))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.UserImage))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User.DateOfBirth))
                .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.GenderName))
                .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
                .ForMember(dest => dest.DevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((DeveloperStatus)src.Status)))
                .ForMember(dest => dest.UserStatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(UserStatus), src.User.Status)))
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.DeveloperSkills.Select(s => s.Skill)))
                .ForMember(dest => dest.Types, opt => opt.MapFrom(src => src.DeveloperTypes.Select(t => t.Type)));

            CreateMap<Developer, GetDevDTO>()
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.DevStatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(DeveloperStatus), src.Status)));

            CreateMap<Developer, GetAllFieldDev>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.UserImage))
                .ForMember(dest => dest.RoleString, opt => opt.MapFrom(src => src.User.Role.RoleName))
                .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
                .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.Level.LevelName))
                .ForMember(dest => dest.GenderString, opt => opt.MapFrom(src => src.Gender.GenderName))
                .ForMember(dest => dest.TypeRequireStrings, opt => opt.MapFrom(src => src.DeveloperTypes.Select(dt => dt.Type.TypeName)))
                .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.DeveloperSkills.Select(dt => dt.Skill.SkillName)))
                .ForMember(dest => dest.UserStatus, opt => opt.MapFrom(src => Enum.GetName(typeof(UserStatus), src.User.Status)))
                .ForMember(dest => dest.DevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((DeveloperStatus)src.Status)));

            CreateMap<Developer, GetDeveloperInProject>()
               .ForMember(dest => dest.HiredDeveloperId, opt => opt.MapFrom(src => src.HiredDevelopers.Where(h => h.DeveloperId == src.DeveloperId).SingleOrDefault()!.HiredDeveloperId))
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
               .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.UserImage))
               .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.EmploymentType.EmploymentTypeName))
               .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.Level.LevelName))
               .ForMember(dest => dest.GenderString, opt => opt.MapFrom(src => src.Gender.GenderName))
               .ForMember(dest => dest.TypeRequireStrings, opt => opt.MapFrom(src => src.DeveloperTypes.Select(dt => dt.Type.TypeName)))
               .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.DeveloperSkills.Select(dt => dt.Skill.SkillName)))
               .ForMember(dest => dest.HiredDevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((HiredDeveloperStatus)src.HiredDevelopers.Where(h => h.DeveloperId == src.DeveloperId).SingleOrDefault()!.Status)))
               .ForMember(dest => dest.StartWorkingDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.HiredDevelopers.Where(h => h.DeveloperId == src.DeveloperId).SingleOrDefault()!.Contract.FromDate)))
               .ForMember(dest => dest.EndWorkingDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.HiredDevelopers.Where(h => h.DeveloperId == src.DeveloperId).SingleOrDefault()!.Contract.ToDate)));

            CreateMap<Developer, GetMatchingDev>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.UserImage))
                .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.Level.LevelName))
                .ForMember(dest => dest.TypeRequireStrings, opt => opt.MapFrom(src => src.DeveloperTypes.Select(dt => dt.Type.TypeName)))
                .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.DeveloperSkills.Select(dt => dt.Skill.SkillName)))
                .ForMember(dest => dest.DevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((DeveloperStatus)src.Status)));

            CreateMap<MatchingPercentage, GetMatchingDev>();
            CreateMap<Developer, CreateDevDTO>().ReverseMap();
            CreateMap<Developer, UpdateDevModel>().ReverseMap();
            CreateMap<Developer, UpdateDevByAdmin>().ReverseMap();
        }
    }
}
