using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using WeHire.Application.DTOs.CV;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.CvEnum;

namespace WeHire.Infrastructure.Services.CvServices
{
    public class CvService : ICvService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public CvService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public List<GetCvDetail> GetAllCv(PagingQuery query, SearchCvDTO searchKey)
        {
            IQueryable<Cv> listCv = _unitOfWork.CvRepository.GetAll().Where(c => c.DeveloperId != null).Include(u => u.Developer);

            listCv = listCv.SearchItems(searchKey);

            listCv = listCv.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedListCv = _mapper.Map<List<GetCvDetail>>(listCv);
            return mappedListCv;
        }

        public async Task<GetCvDetail> GetCvByIdAsync(int id)
        {
            var cv = await _unitOfWork.CvRepository.Get(c => c.CvId == id)
                                                   .Include(d => d.Developer)
                                                   .ThenInclude(u => u.User)
                                                   .FirstOrDefaultAsync();
            if (cv == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.CV_FIELD, ErrorMessage.CV_NOT_EXIST);

            var mappedCv = _mapper.Map<GetCvDetail>(cv);
            return mappedCv;
        }

        public async Task<GetCvDetail> CreateCvAsync(CreateCvDTO requestBody)
        {
            if (requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.CV_FIELD, ErrorMessage.NULL_REQUEST_BODY);

           var dev = await HandleDeveloper(requestBody.DeveloperId);

            var newCv = _mapper.Map<Cv>(requestBody);

            newCv.DeveloperId = requestBody.DeveloperId;
            var fullName = $"{dev.User.FirstName}_{dev.User.LastName}";

            if(requestBody.File != null)
                newCv.Src = await _fileService.UploadFileAsync(requestBody.File, fullName, ChildFolderName.CV_FOLDER);

            await _unitOfWork.CvRepository.InsertAsync(newCv);
            await _unitOfWork.SaveChangesAsync();

            var mappedCv = _mapper.Map<GetCvDetail>(newCv);
            return mappedCv;
        }

        private async Task<Developer> HandleDeveloper(int? devId)
        {
            var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == devId)
                                                           .Include(u => u.User)
                                                           .FirstOrDefaultAsync();
            if (dev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);
            return dev;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.CvRepository.GetAll().CountAsync();
            return total;
        }
    }
}
