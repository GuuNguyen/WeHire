using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Level;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.LevelEnum;
using static WeHire.Domain.Enums.SkillEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class LevelProfile : Profile
    {
        public LevelProfile()
        {
            CreateMap<Level, GetLevelDetail>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(LevelStatus), src.Status)))
                .ReverseMap();
            CreateMap<Level, GetLevelDeveloper>().ReverseMap();
            CreateMap<Level, CreateLevelDTO>().ReverseMap();
            CreateMap<Level, UpdateLevelModel>().ReverseMap();
        }
    }
}
