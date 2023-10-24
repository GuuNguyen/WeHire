using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CV;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.CvEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class CvProfile : Profile
    {
        public CvProfile()
        {
            CreateMap<Cv, GetCvDetail>()
                                .ForMember(dest => dest.DevFullName, opt => opt.MapFrom(src => $"{src.Developer.User.FirstName} {src.Developer.User.FirstName}"))
                                .ReverseMap();
            CreateMap<Cv, CreateCvDTO>().ReverseMap();
        }
    }
}
