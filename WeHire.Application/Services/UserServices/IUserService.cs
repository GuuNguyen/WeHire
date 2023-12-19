using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.UserServices
{
    public interface IUserService
    {
        public Task<object> GetUserLoginAsync(int userId);
        public List<GetUserDetail> GetAllUser(int roleId, SearchUserDTO searchKey);       
        public Task<GetUserDetail> CreateEmployeeAsync(CreateEmployeeDTO requestBody);
        public Task<GetUserDetail> UpdateUserAsync(int id, UpdateUserDTO requestBody);
        public Task<GetUserDetail> UpdateUserByAdminAsync(int id, UpdateUserAdminDTO requestBody);
        public Task<GetUserDetail> UpdatePasswordAsync(int id, UpdatePassword requestBody);
        public Task<GetUserDetail> ChangeStatusAsync(int userId);
        public Task<int> GetTotalItemAsync();

        public Task IsExistEmail(string? email);
        public Task IsExistPhoneNumber(string? phoneNumber);
        public Task IsExistPhoneNumberUpdate(string? oldPhoneNumber, string newPhoneNumber);
        public Task IsExistEmailUpdate(string oldEmail, string newEmail);
    }
}
