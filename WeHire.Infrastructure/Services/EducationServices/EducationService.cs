using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Education;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.EducationServices
{
    public class EducationService : IEducationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EducationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetEducationDTO>> GetEducationsByDevIdAsync(int developerId)
        {
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(developerId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);
            var educations = await _unitOfWork.EducationRepository.Get(e => e.DeveloperId == developerId).ToListAsync();
            var mappedEdus = _mapper.Map<List<GetEducationDTO>>(educations);
            return mappedEdus;
        }

        public async Task<GetEducationDTO> CreateEducationAsync(CreateEducationDTO requestBody)
        {
            var newEdu = _mapper.Map<Education>(requestBody);
            await _unitOfWork.EducationRepository.InsertAsync(newEdu);
            await _unitOfWork.SaveChangesAsync();
            var mappedEdu = _mapper.Map<GetEducationDTO>(newEdu);
            return mappedEdu;
        }

        public List<GetEducationByAdmin> GetEducationsByAdmin(PagingQuery query)
        {
            var edus = _unitOfWork.EducationRepository.GetAll().Include(e => e.Developer).ThenInclude(d => d.User).AsQueryable();

            edus = edus.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappeEdu = _mapper.Map<List<GetEducationByAdmin>>(edus);
            return mappeEdu;
        }

        public async Task<GetEducationDTO> UpdateEducationAsync(int educationId, UpdateEducationModel requestBody)
        {
            if(educationId != requestBody.EducationId)
              throw new ExceptionResponse(HttpStatusCode.BadRequest, "educationId", "educationId does not match!!");

            var edu = await _unitOfWork.EducationRepository.GetByIdAsync(educationId);
            var updatedEdu = _mapper.Map(requestBody, edu);

            _unitOfWork.EducationRepository.Update(updatedEdu);
            await _unitOfWork.SaveChangesAsync();

            var mappedEdu = _mapper.Map<GetEducationDTO>(updatedEdu);
            return mappedEdu;   
        }

        public async Task DeleteEducationAsync(int educationId)
        {
            var edu = await _unitOfWork.EducationRepository.GetByIdAsync(educationId);
            await _unitOfWork.EducationRepository.DeleteAsync(educationId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
