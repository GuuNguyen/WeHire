using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Role;
using WeHire.Entity.IRepositories;

namespace WeHire.Infrastructure.Services.RoleServices
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetRoleDTO> GetAllRole()
        {
            var roles = _unitOfWork.RoleRepository.GetAll();
            var mappedRoles = _mapper.Map<List<GetRoleDTO>>(roles);
            return mappedRoles;
        }
    }
}
