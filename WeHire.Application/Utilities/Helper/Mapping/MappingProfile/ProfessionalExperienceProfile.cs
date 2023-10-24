using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProfessionalExperience;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class ProfessionalExperienceProfile : Profile
    {
        public ProfessionalExperienceProfile()
        {
            CreateMap<ProfessionalExperience, GetProfessionalExperience>().ReverseMap();
            CreateMap<ProfessionalExperience, CreateProfessionalExperience>().ReverseMap();
        }
    }
}
