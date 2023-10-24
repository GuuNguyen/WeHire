using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Gender;
using WeHire.Entity.IRepositories;

namespace WeHire.Infrastructure.Services.GenderServices
{
    public class GenderService : IGenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetGenderDTO> GetAllGender()
        {
            var genders = _unitOfWork.GenderRepository.GetAll();
            var mappedGenders = _mapper.Map<List<GetGenderDTO>>(genders);
            return mappedGenders;
        }
    }
}
