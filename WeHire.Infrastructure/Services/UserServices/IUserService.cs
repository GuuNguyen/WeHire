using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.UserServices
{
    public interface IUserService
    {
        public Task<GetUserDetail> GetUserByIdAsync(int id);
        public Task<object> GetUserLoginAsync(int userId);
        public List<GetUserDetail> GetAllUser(int roleId, PagingQuery query, SearchUserDTO searchKey);       
        public Task<GetUserDetail> CreateEmployeeAsync(CreateEmployeeDTO requestBody);
        public Task<GetUserDetail> UpdateUserAsync(int id, UpdateUserDTO requestBody);
        public Task<GetUserDetail> UpdateUserByAdminAsync(int id, UpdateUserAdminDTO requestBody);
        public Task<GetUserDetail> UpdatePasswordAsync(int id, UpdatePassword requestBody);
        public Task<GetUserDetail> ChangeStatusAsync(int id);
        public Task<GetUserDetail> ChangeRoleAsync(ChangeRoleDTO newRole);
        public Task<int> GetTotalItemAsync();
    }
}
