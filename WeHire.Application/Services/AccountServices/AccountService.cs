﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.UserEnum;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Enums;
using AutoMapper;
using WeHire.Application.Utilities.Helper;
using WeHire.Application.Services.FileServices;
using Microsoft.EntityFrameworkCore;
using WeHire.Domain.Entities;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Services.EmailServices;
using WeHire.Infrastructure.IRepositories;
using WeHire.Application.Services.UserServices;

namespace WeHire.Application.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IJwtHelper _jwtHelper;
        private readonly IUserService _userService;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService,
                              IJwtHelper jwtHelper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<object> LoginAsync(LoginDTO userLoginDTO)
        {
            var user = await _unitOfWork.UserRepository.Get(u =>
                                                            u.Email == userLoginDTO.Email &&
                                                            u.Password == userLoginDTO.Password)
                                                       .Include(u => u.Role)
                                                       .SingleOrDefaultAsync();

            if (user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Email or Password", "Email or password is incorrect. Please try again!");

            if (user.Status == (int)UserStatus.InActive)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STATUS_FIELD, "Your account is inactive!");

            var claims = new[]{
                new Claim("Id", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var token = _jwtHelper.generateJwtToken(claims);

            var refreshToken = _jwtHelper.GenerateRefreshToken();
            var expirationDate = ConvertDateTime.ConvertTimeToSEA(new JwtSecurityTokenHandler().ReadJwtToken(token).ValidTo);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = ConvertDateTime.ConvertTimeToSEA(DateTime.Now.AddDays(7));
            await _unitOfWork.SaveChangesAsync();

            dynamic response;
            if (user.RoleId == (int)RoleEnum.Developer)
            {
                var devId = await _unitOfWork.DeveloperRepository.Get(d => d.UserId == user.UserId).Select(d => d.DeveloperId).SingleOrDefaultAsync();
                response = new DevLoginResponse
                {
                    UserId = user.UserId,
                    DevId = devId,
                    Role = user.Role.RoleName,
                    AccessTokenExp = DateTime.Now.AddHours(24),
                    RefreshTokenExp = (DateTime)user.RefreshTokenExpiryTime,
                    AccessToken = token,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                response = new UserLoginResponse
                {
                    UserId = user.UserId,
                    Role = user.Role.RoleName,
                    AccessTokenExp = DateTime.Now.AddHours(24),
                    RefreshTokenExp = (DateTime)user.RefreshTokenExpiryTime,
                    AccessToken = token,
                    RefreshToken = refreshToken
                };
            }
            return response;
        }


        public async Task<RefreshTokenModel> RefreshAsync(RefreshTokenModel requestBody)
        {
            var principal = _jwtHelper.GetPrincipalFromExpiredToken(requestBody.AccessToken);
            var userId = Int32.Parse(principal.FindFirst("Id")!.Value);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                   ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            if (user.RefreshToken != requestBody.RefreshToken || user.RefreshTokenExpiryTime <= ConvertDateTime.ConvertTimeToSEA(DateTime.Now))
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REFRESH_TOKEN, ErrorMessage.REFRESH_TOKEN_FIELD);

            var newAccessToken = _jwtHelper.generateJwtToken(principal.Claims);
            var newRefreshToken = _jwtHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = ConvertDateTime.ConvertTimeToSEA(DateTime.Now.AddDays(7));
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenModel
            {
                AccessTokenExp = DateTime.Now.AddDays(7),
                RefreshTokenExp = (DateTime)user.RefreshTokenExpiryTime,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }


        public async Task<GetUserDetail> HrSignUpAsync(CreateUserDTO requestBody)
        {
            await _userService.IsExistEmail(requestBody.Email);
            await _userService.IsExistPhoneNumber(requestBody.PhoneNumber);

            var user = _mapper.Map<User>(requestBody);

            user.Status = (int)UserStatus.InActive;
            user.RoleId = (int)RoleEnum.HR;
            user.ConfirmationCode = GenerateUniqueConfirmationCode();

            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var message = new Message(new string[] { user.Email }, "Account Verification", string.Empty);
            var fullName = $"{user.FirstName} {user.LastName}";
            await _emailService.SendEmailAsync(fullName, user.UserId, user.ConfirmationCode, message);

            var newUser = _mapper.Map<GetUserDetail>(user);
            return newUser;
        }


        public async Task RevokeAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);
            user.RefreshToken = null;
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateUniqueConfirmationCode()
        {
            Guid confirmationGuid = Guid.NewGuid();
            string confirmationCode = confirmationGuid.ToString("N");
            return confirmationCode;
        }

        public async Task ConfirmEmailAsync(ConfirmEmailDTO requestBody)
        {
            var user = await _unitOfWork.UserRepository.Get(u => u.UserId == requestBody.UserId && 
                                                           u.ConfirmationCode == requestBody.ConfirmationCode &&
                                                           u.Status == (int)UserStatus.InActive)
                                                       .FirstOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);
            user.Status = (int)UserStatus.Active;
            user.ConfirmationCode = null;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
