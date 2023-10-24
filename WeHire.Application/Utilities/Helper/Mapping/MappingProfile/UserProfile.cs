using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Domain.Enums.UserEnum;
using WeHire.Application.DTOs.User;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.Utilities.Helper.ConvertDate;

namespace WeHire.Application.Utilities.Helper.Mapping.MappingProfile
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, GetUserDetail>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(UserStatus), src.Status)))
                .ForMember(dest => dest.RoleString, opt => opt.MapFrom(src => Enum.GetName(typeof(RoleEnum), src.RoleId)));

            CreateMap<User, GetHRLogin>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => ConvertDateTime.ConvertDateToString(src.DateOfBirth)))
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(UserStatus), src.Status)))
                .ForMember(dest => dest.RoleString, opt => opt.MapFrom(src => Enum.GetName(typeof(RoleEnum), src.RoleId)));

            CreateMap<User, GetDevLogin>()
                .ForMember(dest => dest.StatusString, opt => opt.MapFrom(src => Enum.GetName(typeof(UserStatus), src.Status)))
                .ForMember(dest => dest.RoleString, opt => opt.MapFrom(src => Enum.GetName(typeof(RoleEnum), src.RoleId)));

            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, CreateEmployeeDTO>().ReverseMap();
            CreateMap<User, CreateDevDTO>().ReverseMap();
        }
    }
}
