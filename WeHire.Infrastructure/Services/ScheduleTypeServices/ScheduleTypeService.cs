using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ScheduleType;
using WeHire.Entity.IRepositories;

namespace WeHire.Infrastructure.Services.ScheduleTypeServices
{
    public class ScheduleTypeService : IScheduleTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetScheduleTypeDTO> GetAllScheduleType()
        {
            var types = _unitOfWork.ScheduleTypeRepository.GetAll();
            var mappedTypes = _mapper.Map<List<GetScheduleTypeDTO>>(types);
            return mappedTypes;
        }
    }
}
