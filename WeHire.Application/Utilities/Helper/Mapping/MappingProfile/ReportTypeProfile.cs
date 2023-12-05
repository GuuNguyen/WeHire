using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ReportType;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.ContractEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ReportTypeProfile : Profile
    {
        public ReportTypeProfile()
        {
            CreateMap<ReportType, GetReportTypeModel>()
                .ReverseMap();
        }
    }
}
