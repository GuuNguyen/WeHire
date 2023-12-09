using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.UserDevice;

namespace WeHire.Application.Services.UserDeviceServices
{
    public interface IUserDeviceService
    {
        public Task<List<GetUserDevice>> GetUserDeviceByUserId(int userId);
        public Task<GetUserDevice> CreateUserDevice(CreateUserDevice requestBody);
        public Task<string> RemoveUserDevice(int userDeviceId);
    }
}
