using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.AssignTask;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.AssignTaskEnum;
using static WeHire.Domain.Enums.CompanyPartnerEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class AssignTaskProfile : Profile
    {
        public AssignTaskProfile()
        {
            CreateMap<AssignTask, GetAssignTaskDTO>()
                 .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((AssignTaskStatus)src.Status)))
                 .ReverseMap();
            CreateMap<AssignTask, GetAssignTaskDetail>()
                 .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                 .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((AssignTaskStatus)src.Status)))
                 .ReverseMap();
            CreateMap<AssignTask, CreateAssignTaskDTO>().ReverseMap();
        }
    }
}
