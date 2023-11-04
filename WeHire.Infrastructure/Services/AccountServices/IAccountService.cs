using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.User;

namespace WeHire.Infrastructure.Services.AccountServices
{
    public interface IAccountService
    {
        public Task<object> LoginAsync(LoginDTO userLoginDTO);
        public Task<RefreshTokenModel> RefreshAsync(RefreshTokenModel requestBody);
        public Task<GetUserDetail> HrSignUpAsync(CreateUserDTO requestBody);
        public Task RevokeAsync(int userId);
    }
}
