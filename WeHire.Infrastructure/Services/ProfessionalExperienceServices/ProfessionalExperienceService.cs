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
using WeHire.Domain.Entities;
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

        public async Task<List<GetProfessionalExperience>> GetProfessionalExperiencesByDevIdAsync(int developerId)
        {
            var dev = _unitOfWork.DeveloperRepository.GetByIdAsync(developerId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);
            var pros = await _unitOfWork.ProfessionalExperienceRepository.Get(p => p.DeveloperId == developerId).ToListAsync();
            var mappedPro = _mapper.Map<List<GetProfessionalExperience>>(pros);
            return mappedPro;
        }

        public async Task<GetProfessionalExperience> CreateProfessionalExperienceAsync(CreateProfessionalExperience requestBody)
        {
            var newPro = _mapper.Map<ProfessionalExperience>(requestBody);
            await _unitOfWork.ProfessionalExperienceRepository.InsertAsync(newPro);
            await _unitOfWork.SaveChangesAsync();
            var mappedPro = _mapper.Map<GetProfessionalExperience>(newPro);
            return mappedPro;
        }
    }
}
