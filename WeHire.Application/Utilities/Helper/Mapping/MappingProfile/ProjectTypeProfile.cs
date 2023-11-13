using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProjectType;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.ProjectTypeEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ProjectTypeProfile : Profile
    {
        public ProjectTypeProfile()
        {
            CreateMap<ProjectType, GetProjectTypeDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(ProjectTypeStatus), src.Status)))
                .ReverseMap();
            CreateMap<ProjectType, UpdateProjectTypeDTO>().ReverseMap();
        }
    }
}
