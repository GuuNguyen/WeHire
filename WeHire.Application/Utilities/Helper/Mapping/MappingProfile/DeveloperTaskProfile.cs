using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.DeveloperTaskAssignment;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.DeveloperTaskEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class DeveloperTaskProfile : Profile
    {
        public DeveloperTaskProfile()
        {
            CreateMap<DeveloperTaskAssignment, GetDevTaskAssignmentDTO>()
                      .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(DeveloperTaskStatus), src.Status)))
                      .ReverseMap();
        }
    }
}
