using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Report;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.ContractEnum;
using static WeHire.Domain.Enums.ReportEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Report, GetReportModel>()
                    .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.Project.ProjectId))
                    .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.Project.ProjectCode))
                    .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.ProjectName))
                    .ForMember(dest => dest.DeveloperId, opt => opt.MapFrom(src => src.HiredDeveloper.DeveloperId))
                    .ForMember(dest => dest.DeveloperName, opt => opt.MapFrom(src => $"{src.HiredDeveloper.Developer.User.FirstName} {src.HiredDeveloper.Developer.User.LastName}"))
                    .ForMember(dest => dest.DeveloperCode, opt => opt.MapFrom(src => src.HiredDeveloper.Developer.CodeName))
                    .ForMember(dest => dest.ReportTypeTitle, opt => opt.MapFrom(src => src.ReportType.ReportTypeTitle))
                    .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreateAt)))
                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((ReportStatus)src.Status)))
                    .ReverseMap();

            CreateMap<Report, GetReport>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToStringNumber(src.CreateAt)))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((ReportStatus)src.Status)))
                .ReverseMap();
            CreateMap<Report, CreateReportModel>().ReverseMap();
        }
    }
}
