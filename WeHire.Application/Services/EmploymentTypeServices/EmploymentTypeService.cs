using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.EmploymentType;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.EmploymentTypeServices
{
    public class EmploymentTypeService : IEmploymentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmploymentTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetEmploymentTypeDTO> GetAllEmployments()
        {
            var types = _unitOfWork.EmploymentTypeRepository.GetAll();
            var mappedType = _mapper.Map<List<GetEmploymentTypeDTO>>(types);
            return mappedType;
        }
    }
}
