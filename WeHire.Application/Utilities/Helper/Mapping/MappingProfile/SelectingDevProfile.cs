using AutoMapper;
using WeHire.Application.DTOs.SelectingDev;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class SelectingDevProfile : Profile
    {
        public SelectingDevProfile()
        {
            CreateMap<SelectedDev, GetSelectingDevDTO>()
            .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((SelectedDevStatus)src.Status)))
            .ReverseMap();

            CreateMap<Developer, GetSelectedDevDetail>()
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
               .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.UserImage))
               .ForMember(dest => dest.LevelRequireName, opt => opt.MapFrom(src => src.Level.LevelName))
               .ForMember(dest => dest.TypeRequireStrings, opt => opt.MapFrom(src => src.DeveloperTypes.Select(dt => dt.Type.TypeName)))
               .ForMember(dest => dest.SkillRequireStrings, opt => opt.MapFrom(src => src.DeveloperSkills.Select(dt => dt.Skill.SkillName)))
               .ForMember(dest => dest.SelectedDevStatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((SelectedDevStatus)src.Status)));
        }
    }
}
