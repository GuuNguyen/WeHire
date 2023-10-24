using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Education;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class EducationProfile : Profile
    {
        public EducationProfile()
        {
            CreateMap<Education, GetEducationDTO>().ReverseMap();
            CreateMap<Education, CreateEducationDTO>().ReverseMap();
        }
    }
}
