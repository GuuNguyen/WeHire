using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ScheduleType;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.ScheduleTypeEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<ScheduleType, GetScheduleTypeDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(ScheduleTypeStatus), src.Status)))
                .ReverseMap();
        }
    }
}
