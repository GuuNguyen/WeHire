using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProfessionalExperience;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.ProfessionalExperienceServices
{
    public class ProfessionalExperienceService : IProfessionalExperienceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProfessionalExperienceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetPEByAdmin> GetProfessionalExperiencesAsync(PagingQuery query)
        {
            var profess = _unitOfWork.ProfessionalExperienceRepository.GetAll().Include(p => p.Developer).ThenInclude(d => d.User).AsQueryable();

            profess = profess.PagedItems(query.PageIndex ,query.PageSize).AsQueryable();
            var mappedProfess = _mapper.Map<List<GetPEByAdmin>>(profess);
            return mappedProfess;
        }

        public async Task<List<GetProfessionalExperience>> GetProfessionalExperiencesByDevIdAsync(int developerId)
        {
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(developerId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            var pros = await _unitOfWork.ProfessionalExperienceRepository.Get(p => p.DeveloperId == developerId).ToListAsync();

            var mappedPro = _mapper.Map<List<GetProfessionalExperience>>(pros);
            return mappedPro;
        }

        public async Task<GetProfessionalExperience> CreateProfessionalExperienceAsync(CreateProfessionalExperience requestBody)
        {
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(requestBody.DeveloperId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            var newPro = _mapper.Map<ProfessionalExperience>(requestBody);
            await _unitOfWork.ProfessionalExperienceRepository.InsertAsync(newPro);
            await _unitOfWork.SaveChangesAsync();
            var mappedPro = _mapper.Map<GetProfessionalExperience>(newPro);
            return mappedPro;
        }
       
        public async Task<GetProfessionalExperience> UpdateProfessionalExperienceAsync(int professionalExperienceId, UpdateProfessionalExperience requestBody)
        {
            if(professionalExperienceId != requestBody.ProfessionalExperienceId)
              throw new ExceptionResponse(HttpStatusCode.BadRequest, "professionalExperienceId", "professionalExperienceId not match!");
            
            var profess = await _unitOfWork.ProfessionalExperienceRepository.GetByIdAsync(professionalExperienceId)
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "ProfessionalExperience", "ProfessionalExperience does not exist!!");
            
            var updatedProfess = _mapper.Map(requestBody, profess);
            _unitOfWork.ProfessionalExperienceRepository.Update(updatedProfess);
            await _unitOfWork.SaveChangesAsync();

            var mappedProfess = _mapper.Map<GetProfessionalExperience>(updatedProfess);
            return mappedProfess;
        }

        public async Task DeleteProfessionalExperienceAsync(int professionalExperienceId)
        {
            var profess = await _unitOfWork.ProfessionalExperienceRepository.GetByIdAsync(professionalExperienceId)
             ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "ProfessionalExperience", "ProfessionalExperience does not exist!!");

            await _unitOfWork.ProfessionalExperienceRepository.DeleteAsync(professionalExperienceId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
