using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Role;
using WeHire.Domain.Entities;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, GetRoleDTO>().ReverseMap();
        }
    }
}
