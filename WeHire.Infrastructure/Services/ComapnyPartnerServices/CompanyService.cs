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
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.CompanyPartnerEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Infrastructure.Services.ComapnyPartnerServices
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

        public List<GetCompanyDetail> GetCompany(PagingQuery query, SearchCompanyDTO searchKey)
        {
            var company = _unitOfWork.CompanyRepository.GetAll().Include(u => u.User).AsQueryable();

            company = company.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

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

            var isExistEmail = await IsExistEmailAsync(requestBody.CompanyEmail);

            if(isExistEmail)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);

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

            var updateCompany = _mapper.Map(requestBody, company);
            if(requestBody.File != null)
                updateCompany.CompanyImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.CompanyName, ChildFolderName.LOGO_FOLDER);

            _unitOfWork.CompanyRepository.Update(updateCompany);
            await _unitOfWork.SaveChangesAsync();

            var mappedCompany = _mapper.Map<GetCompanyDTO>(updateCompany);
            return mappedCompany;
        }

        private async Task<bool> IsExistEmailAsync(string email)
        {
            if(string.IsNullOrEmpty(email))
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.INCORRECT_INFO);
            var isExistEmail = await _unitOfWork.CompanyRepository.AnyAsync(c => c.CompanyEmail == email);

            return isExistEmail;
        }

        private async Task IsExistCompany(int? userId)
        {
            var isExist = await _unitOfWork.CompanyRepository.AnyAsync(c => c.UserId == userId);
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, "Company is existed");
        }

        public Task DeleteCompanyAsync(int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.CompanyRepository.GetAll().CountAsync();
            return total;
        }
    }
}
