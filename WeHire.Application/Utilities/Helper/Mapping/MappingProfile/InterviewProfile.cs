using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Interview;
using WeHire.Application.Utilities.Helper.EnumDescription;
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
                                    .ForMember(dest => dest.InterviewerName, opt => opt.MapFrom(src => $"{src.Interviewer.FirstName} {src.Interviewer.LastName}"))
                                    .ForMember(dest => dest.AssignStaffName, opt => opt.MapFrom(src => $"{src.AssignStaff.FirstName} {src.AssignStaff.LastName}"))
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, GetInterviewWithDev>()
                                    .ForMember(dest => dest.InterviewerName, opt => opt.MapFrom(src => $"{src.Interviewer.FirstName} {src.Interviewer.LastName}"))
                                    .ForMember(dest => dest.AssignStaffName, opt => opt.MapFrom(src => $"{src.AssignStaff.FirstName} {src.AssignStaff.LastName}"))
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, GetInterviewDTO>()
                                    .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((InterviewStatus)src.Status)))
                                    .ReverseMap();
            CreateMap<Interview, CreateInterviewDTO>().ReverseMap();
        }
    }
}
