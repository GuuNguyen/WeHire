using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.ContractEnum;
using static WeHire.Domain.Enums.DeveloperEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ContractProfile : Profile
    {
        public ContractProfile()
        {
            CreateMap<Contract, GetContractDTO>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => EnumHelper.GetEnumDescription((ContractStatus)src.Status)))
                .ReverseMap();
        }
    }
}
