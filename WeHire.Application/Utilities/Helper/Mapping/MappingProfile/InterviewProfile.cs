using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Interview;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.InterviewEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class InterviewProfile : Profile
    {
        public InterviewProfile()
        {
            CreateMap<Interview, GetInterviewDetail>()
                                    .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreateAt)))
                                    .ForMember(dest => dest.DateOfInterview, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.DateOfInterview)))
                                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.StartTime)))
                                    .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.EndTime)))
                                    //.ForMember(dest => dest.InterviewerName, opt => opt.MapFrom(src => $"{src.Interviewer.FirstName} {src.Interviewer.LastName}"))
                                    //.ForMember(dest => dest.AssignStaffName, opt => opt.MapFrom(src => $"{src.AssignStaff.FirstName} {src.AssignStaff.LastName}"))
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, GetInterviewWithDev>()
                                    .ForMember(dest => dest.PostedTime, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.CreateAt)))
                                    .ForMember(dest => dest.DateOfInterview, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.DateOfInterview)))
                                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.StartTime)))
                                    .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.EndTime)))
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, GetInterviewDTO>()
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, CreateInterviewDTO>().ReverseMap();
        }
    }
}
