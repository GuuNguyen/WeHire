using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ReportType;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.ReportTypeServices
{
    public class ReportTypeService : IReportTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetReportTypeModel> GetReportTypes()
        {
            var reportTypes = _unitOfWork.ReportTypeRepository.GetAll();
            var mappedReportTypes = _mapper.Map<List<GetReportTypeModel>>(reportTypes);
            return mappedReportTypes;
        }
    }
}
