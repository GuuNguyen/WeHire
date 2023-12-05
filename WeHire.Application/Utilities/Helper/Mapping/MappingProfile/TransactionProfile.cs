using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.PostedTime;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using static WeHire.Domain.Enums.TransactionEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, GetTransactionDTO>()
                        .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.PayPeriod.Project.Company.CompanyName))
                        .ForMember(dest => dest.CompanyImage, opt => opt.MapFrom(src => src.PayPeriod.Project.Company.CompanyImage))
                        .ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.PayPeriod.Project.ProjectName))
                        .ForMember(dest => dest.ProjectCode, opt => opt.MapFrom(src => src.PayPeriod.Project.ProjectCode))
                        .ForMember(dest => dest.PayForMonth, opt => opt.MapFrom(src => src.PayPeriod.EndDate.Value.ToString("MMMM yyyy")))
                        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToString("#,##0 VND")))
                        .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.Timestamp)))
                        .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(TransactionStatus), src.Status)))
                        .ReverseMap();
            CreateMap<Transaction, CreateTransactionDTO>().ReverseMap();
        }
    }
}
