using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private readonly IJwtHelper _jwtHelper;
        private readonly IFileService _fileService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper,
                           IJwtHelper jwtHelper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _fileService = fileService;
        }

        public async Task<object> LoginAsync(LoginDTO userLoginDTO)
        {
            var user = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u =>
                        u.Email == userLoginDTO.Email &&
                        u.Password == userLoginDTO.Password);

            if (user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Email or Password", "Email or password is incorrect. Please try again!");

            if (user.Status == (int)UserEnum.UserStatus.InActive)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STATUS_FIELD, "Your account is inactive!");

            var role = await _unitOfWork.RoleRepository.GetFirstOrDefaultAsync(r => r.RoleId == user.RoleId);

            var token = _jwtHelper.generateJwtToken(role, user.UserId);
            dynamic responseObj = new
            {
                UserId = user.UserId,
                Role = role.RoleName,
                Token = token
            };
            if (user.RoleId == (int)RoleEnum.Developer)
            {
                var devId = await _unitOfWork.DeveloperRepository.Get(d => d.UserId == user.UserId).Select(d => d.DeveloperId).SingleOrDefaultAsync();
                responseObj = new
                {
                    UserId = user.UserId,
                    DevId = devId,
                    Role = role.RoleName,
                    Token = token
                };
            }
            return responseObj;
        }

        public async Task<GetUserDetail> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            if (user.RoleId == (int)RoleEnum.HR)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.ROLE_FIELD, ErrorMessage.NOT_ALLOWS);

            var userDetail = _mapper.Map<GetUserDetail>(user);
            return userDetail;
        }

        public List<GetUserDetail> GetStaff(PagingQuery query)
        {
            var staffs = _unitOfWork.UserRepository.Get(u => u.RoleId == (int)RoleEnum.Staff && u.Status == (int)UserStatus.Active);

            staffs = staffs.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedStaffs = _mapper.Map<List<GetUserDetail>>(staffs);
            return mappedStaffs;
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


        public List<GetUserDetail> GetAllUser(PagingQuery query, SearchUserDTO searchKey)
        {
            var users = _unitOfWork.UserRepository.GetAll();

            users = users.SearchItems(searchKey);

            users = users.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var userDetailsList = _mapper.Map<List<GetUserDetail>>(users);

            return userDetailsList;
        }

        public async Task<GetUserDetail> CreateUserAsync(CreateUserDTO requestBody)
        {
            var user = _mapper.Map<User>(requestBody);
            var isExitedEmail = await _unitOfWork.UserRepository.AnyAsync(u => u.Email == user.Email);

            if (user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.INCORRECT_INFO);

            if (isExitedEmail)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);

            user.Status = (int)UserStatus.Active;
            user.RoleId = (int)RoleEnum.HR;

            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var newUser = _mapper.Map<GetUserDetail>(user);
            return newUser;
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

        public async Task<GetUserDetail> ChangeStatusAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
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

        public async Task<GetUserDetail> ChangeRoleAsync(ChangeRoleDTO newRole)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(newRole.UserId);

            if (user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            if (!Enum.IsDefined(typeof(RoleEnum), newRole.Role))
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STATUS_FIELD, $"Role {newRole.Role} is not exist!");

            user.RoleId = newRole.Role;
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
