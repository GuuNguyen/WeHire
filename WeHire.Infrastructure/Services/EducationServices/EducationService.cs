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
            var dev = _unitOfWork.DeveloperRepository.GetByIdAsync(developerId)
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
    }
}
