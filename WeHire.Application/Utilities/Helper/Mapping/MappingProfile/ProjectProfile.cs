using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.ProjectEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, GetProjectDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(ProjectStatus), src.Status)))
                .ReverseMap();
            CreateMap<Project, GetListProjectDTO>()
                .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.Company.CompanyImage))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                .ForMember(dest => dest.ProjectTypeName, opt => opt.MapFrom(src => src.ProjectType.ProjectTypeName))
                .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((ProjectStatus)src.Status)))
                .ReverseMap();
            CreateMap<Project, GetProjectDetail>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.ProjectTypeName, opt => opt.MapFrom(src => src.ProjectType.ProjectTypeName))
                .ForMember(dest => dest.StartDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.StartDate)))
                .ForMember(dest => dest.EndDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.EndDate)))
                .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreatedAt)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.CreatedAt)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.UpdatedAt)))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((ProjectStatus)src.Status)))
                .ReverseMap();
            CreateMap<Project, CreateProjectDTO>().ReverseMap();
            CreateMap<Project, UpdateProjectDTO>().ReverseMap();
        }
    }
}
