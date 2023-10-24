using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.EmploymentType;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.EmploymentTypeEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class EmploymentTypeProfile : Profile
    {
        public EmploymentTypeProfile()
        {
            CreateMap<EmploymentType, GetEmploymentTypeDTO>()
                  .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(EmploymentTypeStatus), src.Status)))
                  .ReverseMap();
        }
    }
}
