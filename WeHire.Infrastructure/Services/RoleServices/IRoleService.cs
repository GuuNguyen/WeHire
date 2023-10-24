using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Role;

namespace WeHire.Infrastructure.Services.RoleServices
{
    public interface IRoleService
    {
        public List<GetRoleDTO> GetAllRole();
    }
}
