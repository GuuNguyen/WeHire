using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.WorkLog;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class WorkLogProfile : Profile
    {
        public WorkLogProfile()
        {
            CreateMap<WorkLog, GetWorkLogModel>()
                .ForMember(dest => dest.WorkDateMMM, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.WorkDate)))
                .ForMember(dest => dest.TimeIn, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.TimeIn)))
                .ForMember(dest => dest.TimeOut, opt => opt.MapFrom(src => ConvertTime.ConvertTimeToShortFormat(src.TimeOut)))
                .ForMember(dest => dest.HourWorkInDay, opt => opt.MapFrom(src => Math.Round(ConvertTime.CalculateTotalWorkTime(src.TimeIn, src.TimeOut), 1)))
                .ReverseMap();

            CreateMap<WorkLog, UpdateWorkLogModel>().ReverseMap();
        }
    }
}
