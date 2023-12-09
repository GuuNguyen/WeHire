using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProjectType;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Infrastructure.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.ProjectTypeEnum;

namespace WeHire.Application.Services.ProjectTypeServices
{
    public class ProjectTypeService : IProjectTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProjectTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetProjectTypeDTO> GetAllProjectTypeAsync(PagingQuery query, SearchProjectType searchKey)
        {
            var projectTypes = _unitOfWork.ProjectTypeRepository.GetAll();

            projectTypes = projectTypes.SearchItems(searchKey);

            projectTypes = projectTypes.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedProjectType = _mapper.Map<List<GetProjectTypeDTO>>(projectTypes);
            return mappedProjectType;
        }

        public async Task<GetProjectTypeDTO> CreateProjectTypeAsync(string projectTypeName)
        {
            var newProjectType = new ProjectType
            {
                ProjectTypeName = projectTypeName,
                Status = (int)ProjectTypeStatus.Active
            };
            await _unitOfWork.ProjectTypeRepository.InsertAsync(newProjectType);
            await _unitOfWork.SaveChangesAsync();

            var mappedProjectType = _mapper.Map<GetProjectTypeDTO>(newProjectType);
            return mappedProjectType;
        }

        public async Task<GetProjectTypeDTO> UpdateProjectTypeAsync(int projectTypeId, UpdateProjectTypeDTO requestBody)
        {
            if(projectTypeId != requestBody.ProjectTypeId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_TYPE_FIELD, ErrorMessage.PROJECT_TYPE_NOT_MATCH);
            var projectType = await _unitOfWork.ProjectTypeRepository.GetByIdAsync(projectTypeId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_TYPE_FIELD, ErrorMessage.PROJECT_TYPE_NOT_EXIST);
            var updateProjectType = _mapper.Map(requestBody, projectType);
            _unitOfWork.ProjectTypeRepository.Update(updateProjectType);

            var mappedProjectType = _mapper.Map<GetProjectTypeDTO>(projectType);
            return mappedProjectType;
        }

        public async Task DeleteProjectTypeAsync(int projectTypeId)
        {
            var projectType = await _unitOfWork.ProjectTypeRepository.GetByIdAsync(projectTypeId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_TYPE_FIELD, ErrorMessage.PROJECT_TYPE_NOT_EXIST);
            projectType.Status = (int)ProjectTypeStatus.Inactive;
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<int> GetTotalItemAsync()
        {
            var total = _unitOfWork.ProjectTypeRepository.Get().AsNoTracking().CountAsync();
            return total;
        }
    }
}
