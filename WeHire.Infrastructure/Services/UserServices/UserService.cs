using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Application.Utilities.Helper;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Infrastructure.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<object> GetUserLoginAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.UserId == userId).SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            object userDetail = null!;

            switch ((RoleEnum)user.RoleId!)
            {
                case RoleEnum.HR:
                    var hrLogin = _mapper.Map<GetHRLogin>(user);
                    var company = _unitOfWork.CompanyRepository.Get(c => c.UserId == userId).SingleOrDefault();
                    hrLogin.CompanyId = company != null ? company.CompanyId : null;
                    userDetail = hrLogin;
                    break;
                case RoleEnum.Unofficial:
                case RoleEnum.Developer:
                    var dev = await _unitOfWork.DeveloperRepository.Get(d => d.UserId == userId).SingleOrDefaultAsync();
                    if (dev == null)
                        throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

                    var devLogin = _mapper.Map<GetDevLogin>(user);
                    devLogin.DeveloperId = dev.DeveloperId;
                    userDetail = devLogin;
                    break;
                default:
                    userDetail = _mapper.Map<GetUserDetail>(user);
                    break;
            }
            return userDetail;
        }


        public List<GetUserDetail> GetAllUser(int roleId, PagingQuery query, SearchUserDTO searchKey)
        {
            var users = _unitOfWork.UserRepository.Get(u => u.RoleId == roleId && u.RoleId != (int)RoleEnum.Developer).AsNoTracking();

            users = users.SearchItems(searchKey);

            users = users.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var userDetailsList = _mapper.Map<List<GetUserDetail>>(users);

            return userDetailsList;
        }


        public async Task<GetUserDetail> CreateEmployeeAsync(CreateEmployeeDTO requestBody)
        {
            if (requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            var user = _mapper.Map<User>(requestBody);
            var isExitedEmail = await _unitOfWork.UserRepository.AnyAsync(u => u.Email == user.Email);

            if (isExitedEmail)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);

            user.Status = (int)UserStatus.Active;

            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var newUser = _mapper.Map<GetUserDetail>(user);
            return newUser;
        }

        public async Task<GetUserDetail> UpdateUserAsync(int id, UpdateUserDTO requestBody)
        {
            if (id != requestBody.UserId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_ID_FIELD, ErrorMessage.USER_NOT_EXIST);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            user = _mapper.Map(requestBody, user);
            if(requestBody.File != null)
                user.UserImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.FirstName + requestBody.LastName, ChildFolderName.AVATAR_FOLDER);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDetail = _mapper.Map<GetUserDetail>(user);
            return userDetail;
        }

        public async Task<GetUserDetail> UpdateUserByAdminAsync(int id, UpdateUserAdminDTO requestBody)
        {
            if (id != requestBody.UserId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_ID_FIELD, ErrorMessage.USER_NOT_EXIST);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            user = _mapper.Map(requestBody, user);
            if (requestBody.File != null)
                user.UserImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.FirstName + requestBody.LastName, ChildFolderName.AVATAR_FOLDER);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDetail = _mapper.Map<GetUserDetail>(user);
            return userDetail;
        }

        public async Task<GetUserDetail> UpdatePasswordAsync(int id, UpdatePassword requestBody)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_ID_FIELD, ErrorMessage.USER_NOT_EXIST);

            if (!user.Password.Equals(requestBody.CurrentPassword))
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PASSWORD_FIELD, ErrorMessage.CURRENT_PASSWORD_NOT_MATCH);

            if (!requestBody.NewPassword.Equals(requestBody.ConfirmPassword))
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PASSWORD_FIELD, ErrorMessage.NEW_PASSWORD_NOT_MATCH);

            user.Password = requestBody.NewPassword;
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDetail = _mapper.Map<GetUserDetail>(user);
            return userDetail;
        }

        public async Task<GetUserDetail> ChangeStatusAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            if (user.Status == (int)UserStatus.Active)
            {
                user.Status = (int)UserStatus.InActive;
            }
            else
            {
                user.Status = (int)UserStatus.Active;
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var mappedUser = _mapper.Map<GetUserDetail>(user);
            return mappedUser;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.UserRepository.GetAll().CountAsync();
            return total;
        }
    }
}
