using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Type;
using static WeHire.Domain.Enums.SkillEnum;
using static WeHire.Domain.Enums.TypeEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class TypeProfile : Profile
    {
        public TypeProfile()
        {
            CreateMap<Domain.Entities.Type, GetTypeDetail>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(TypeStatus), src.Status)))
                .ReverseMap();
            CreateMap<Domain.Entities.Type, CreateTypeDTO>().ReverseMap();
        }
    }
}
