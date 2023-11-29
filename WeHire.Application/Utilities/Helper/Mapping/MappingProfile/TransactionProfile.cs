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
                        .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToString("#,##0 VND")))
                        .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => PostedTimeCalculateHelper.GetElapsedTimeSinceCreation(src.Timestamp)))
                        .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(TransactionStatus), src.Status)))
                        .ReverseMap();
            CreateMap<Transaction, CreateTransactionDTO>().ReverseMap();
        }
    }
}
