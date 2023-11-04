using AutoMapper;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.UserDevice;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.UserDeviceServices
{
    public class UserDeviceService : IUserDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserDeviceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetUserDevice>> GetUserDeviceByUserId(int userId)
        {
            var userDevice = await _unitOfWork.UserDeviceRepository.Get(u => u.UserId == userId).ToListAsync();
            var mappedUserDevice = _mapper.Map<List<GetUserDevice>>(userDevice);
            return mappedUserDevice;
        }

        public async Task<GetUserDevice> CreateUserDevice(CreateUserDevice requestBody)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(requestBody.UserId!)
                   ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            await IsExistDeviceAsync(requestBody.UserId, requestBody.DeviceToken);

            var newUserDevice = _mapper.Map<UserDevice>(requestBody);
            await _unitOfWork.UserDeviceRepository.InsertAsync(newUserDevice);
            await _unitOfWork.SaveChangesAsync();   

            var mappedUserDevice = _mapper.Map<GetUserDevice>(newUserDevice); 
            return mappedUserDevice;
        }

        public async Task<string> RemoveUserDevice(int userDeviceId)
        {
            var token = await _unitOfWork.UserDeviceRepository.GetByIdAsync(userDeviceId)
                   ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_DEVICE, ErrorMessage.USER_DEVICE_NOT_EXIST);

            await _unitOfWork.UserDeviceRepository.DeleteAsync(token.UserDeviceId);
            await _unitOfWork.SaveChangesAsync();
            return "Remove success!";
        }

        private async Task IsExistDeviceAsync(int userId, string deviceToken)
        {
            var userDevice = await _unitOfWork.UserDeviceRepository.Get(ud => ud.UserId == userId && ud.DeviceToken == deviceToken).SingleOrDefaultAsync();
            if (userDevice != null)
            {
                await _unitOfWork.UserDeviceRepository.DeleteAsync(userDevice.UserDeviceId);
                await _unitOfWork.SaveChangesAsync();
            }             
        }
    }
}
