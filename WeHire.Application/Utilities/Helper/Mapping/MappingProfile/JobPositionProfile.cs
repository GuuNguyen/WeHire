using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.JobPosition;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.JobPositionEnum;
using static WeHire.Domain.Enums.ProjectEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class JobPositionProfile : Profile
    {
        public JobPositionProfile()
        {
            CreateMap<JobPosition, GetJobPosition>()
                   .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(JobPositionStatus), src.Status)))
                   .ReverseMap();
            CreateMap<JobPosition, CreateJobPosition>().ReverseMap();
            CreateMap<JobPosition, UpdateJobPosition>().ReverseMap();
        }
    }
}
