using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Skill;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.SkillEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class SkillProfile : Profile
    {
        public SkillProfile()
        {
            CreateMap<Skill, GetSkillDetail>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(SkillStatus), src.Status)))
                .ReverseMap();
            CreateMap<Skill, GetSkillDeveloper>().ReverseMap();
            CreateMap<Skill, CreateSkillDTO>().ReverseMap();
            CreateMap<Skill, UpdateSkillModel>().ReverseMap();
        }
    }
}
