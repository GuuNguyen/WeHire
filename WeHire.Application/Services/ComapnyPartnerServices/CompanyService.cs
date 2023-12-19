using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Application.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.CompanyPartnerEnum;
using static WeHire.Domain.Enums.UserEnum;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.ComapnyPartnerServices
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public List<GetCompanyDetail> GetCompany(SearchCompanyDTO searchKey)
        {
            var company = _unitOfWork.CompanyRepository.GetAll().Include(u => u.User).AsQueryable();

            company = company.SearchItems(searchKey);

            var mappedCompany = _mapper.Map<List<GetCompanyDetail>>(company);

            return mappedCompany;
        }

        public async Task<GetCompanyDetail> GetCompanyById(int id)
        {
            var company = await _unitOfWork.CompanyRepository
                                        .Get(c => c.CompanyId == id)
                                        .Include(u => u.User)
                                        .FirstOrDefaultAsync();

            if(company == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var mappedCompany = _mapper.Map<GetCompanyDetail>(company);
            return mappedCompany;
        }

        public async Task<GetCompanyDTO> CreateCompanyAsync(CreateCompanyDTO requestBody)
        {
            await IsExistCompany(requestBody.UserId);
            var user = await _unitOfWork.UserRepository.GetByIdAsync(requestBody.UserId);

            if(user == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.USER_FIELD, ErrorMessage.USER_NOT_EXIST);

            if(user.Status == (int)UserStatus.InActive)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STATUS_FIELD, ErrorMessage.NO_LONGER_ACTIVE);

            if(user.RoleId != (int)RoleEnum.HR)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.ROLE_FIELD, ErrorMessage.NOT_ALLOWS);

            await IsExistEmail(requestBody.CompanyEmail);
            await IsExistPhoneNumber(requestBody.PhoneNumber);

            var newCompany = _mapper.Map<CompanyPartner>(requestBody);
            newCompany.Rating = 0;
            newCompany.Status = (int)CompanyStatus.Active;
            newCompany.CompanyImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.CompanyName, ChildFolderName.LOGO_FOLDER);

            await _unitOfWork.CompanyRepository.InsertAsync(newCompany);
            await _unitOfWork.SaveChangesAsync();

            var mappedNewCompany = _mapper.Map<GetCompanyDTO>(newCompany);
            return mappedNewCompany;
        }

        public async Task<GetCompanyDTO> UpdateCompanyAsync(int companyId, UpdateCompanyDTO requestBody)
        {
            if(companyId != requestBody.CompanyId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            await IsExistEmailUpdate(company.CompanyEmail, requestBody.CompanyEmail);
            await IsExistPhoneNumberUpdate(company.PhoneNumber, requestBody.PhoneNumber);
            
            var updateCompany = _mapper.Map(requestBody, company);
            if(requestBody.File != null)
                updateCompany.CompanyImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.CompanyName, ChildFolderName.LOGO_FOLDER);

            _unitOfWork.CompanyRepository.Update(updateCompany);
            await _unitOfWork.SaveChangesAsync();

            var mappedCompany = _mapper.Map<GetCompanyDTO>(updateCompany);
            return mappedCompany;
        }

        public async Task IsExistPhoneNumber(string? phoneNumber)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(u => u.PhoneNumber.Equals(phoneNumber));
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PHONE_NUMBER_FIELD, ErrorMessage.PHONE_NUMBER_ALREADY_EXIST);
        }

        public async Task IsExistEmail(string? email)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(u => u.CompanyEmail.Equals(email));
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);
        }

        public async Task IsExistPhoneNumberUpdate(string? oldPhoneNumber, string newPhoneNumber)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(u => u.PhoneNumber.Equals(newPhoneNumber) && oldPhoneNumber != newPhoneNumber);
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PHONE_NUMBER_FIELD, ErrorMessage.PHONE_NUMBER_ALREADY_EXIST);
        }

        public async Task IsExistEmailUpdate(string oldEmail, string newEmail)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(u => u.CompanyEmail.Equals(newEmail) && !oldEmail.Equals(newEmail));
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);
        }

        private async Task IsExistCompany(int? userId)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(c => c.UserId == userId);
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, "Company is existed");
        }

        public async Task DeleteCompanyAsync(int companyId)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);
            company.Status = (int)CompanyStatus.Inactive;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.CompanyRepository.GetAll().CountAsync();
            return total;
        }
    }
}
