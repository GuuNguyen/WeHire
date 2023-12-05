using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiredDeveloper;
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
        }
    }
}
