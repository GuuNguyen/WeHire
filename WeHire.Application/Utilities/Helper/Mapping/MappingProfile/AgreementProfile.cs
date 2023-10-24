using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Agreement;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.AgreementEnum;
using static WeHire.Domain.Enums.InterviewEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class AgreementProfile : Profile
    {
        public AgreementProfile()
        {
            CreateMap<Agreement, GetAgreementDTO>()
                             .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((AgreementStatus)src.Status)))
                             .ReverseMap();
            CreateMap<Agreement, CreateAgreementDTO>().ReverseMap();
        }
    }
}
