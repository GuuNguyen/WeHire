using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Gender;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class GenderProfile : Profile
    {
        public GenderProfile()
        {
            CreateMap<Gender, GetGenderDTO>().ReverseMap();
        }
    }
}
