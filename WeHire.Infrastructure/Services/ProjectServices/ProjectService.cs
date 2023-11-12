using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.ProjectEnum;

namespace WeHire.Infrastructure.Services.ProjectServices
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public List<GetListProjectDTO> GetAllProject(PagingQuery query, SearchProjectDTO searchKey)
        {
            IQueryable<Project> projects = _unitOfWork.ProjectRepository.GetAll()
                                                        .Include(p => p.Company)
                                                        .Include(p => p.ProjectType)
                                                        .OrderByDescending(n => n.CreatedAt);
            projects = projects.SearchItems(searchKey);
            projects = projects.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedProjects = _mapper.Map<List<GetListProjectDTO>>(projects);
            return mappedProjects;
        }

        public List<GetListProjectDTO> GetAllProjectByCompanyId(int companyId, PagingQuery query, SearchProjectDTO searchKey)
        {
            IQueryable<Project> projects = _unitOfWork.ProjectRepository.Get(p => p.CompanyId == companyId)
                                                      .Include(p => p.Company)
                                                      .Include(p => p.ProjectType)
                                                      .OrderByDescending(n => n.CreatedAt);
            projects = projects.SearchItems(searchKey);
            projects = projects.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedProjects = _mapper.Map<List<GetListProjectDTO>>(projects);
            return mappedProjects;
        }

        public async Task<GetProjectDetail> GetProjectById(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId)
                                                      .Include(p => p.Company)
                                                      .Include(p => p.ProjectType)
                                                      .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_EXIST);

            var mappedProject = _mapper.Map<GetProjectDetail>(project);
            return mappedProject;
        }

        public async Task<GetProjectDTO> CreateProjectAsync(CreateProjectDTO requestBody)
        {
            var isExistCompany = await _unitOfWork.CompanyRepository.AnyAsync(c => c.CompanyId == requestBody.CompanyId);

            if(!isExistCompany)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var newProject = _mapper.Map<Project>(requestBody);
            newProject.ProjectCode = await GenerateUniqueCodeName();
            newProject.CreatedAt = DateTime.Now;
            newProject.NumberOfDev = 0;
            newProject.Status = (int)ProjectStatus.Preparing;
            if (requestBody.File != null)
                newProject.BackgroundImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.ProjectName, ChildFolderName.BACKGROUND_FOLDER);
            await _unitOfWork.ProjectRepository.InsertAsync(newProject);
            await _unitOfWork.SaveChangesAsync();

            var mappedProject = _mapper.Map<GetProjectDTO>(newProject);
            return mappedProject;
        }

        public async Task<GetProjectDTO> UpdateProjectAsync(int projectId, UpdateProjectDTO requestBody)
        {
            if(projectId != requestBody.ProjectId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_MATCH);

            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var updateProject = _mapper.Map(requestBody, project);

            if (requestBody.File != null)
                updateProject.BackgroundImage = await _fileService.UploadFileAsync(requestBody.File!, requestBody.ProjectName, ChildFolderName.BACKGROUND_FOLDER);
            
            updateProject.UpdatedAt = DateTime.Now;
            _unitOfWork.ProjectRepository.Update(updateProject);

            await _unitOfWork.SaveChangesAsync();

            var mappedProject = _mapper.Map<GetProjectDTO>(updateProject);
            return mappedProject;
        }

        public async Task<int> GetTotalProjectAsync(int? companyId = null)
        {
            var query = _unitOfWork.ProjectRepository.GetAll();

            if(companyId.HasValue)
                query.Where(p => p.CompanyId == companyId);

            return await query.CountAsync();
        }

        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistDevCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "PRJ" + randomNumber.ToString();
                isExistDevCode = await _unitOfWork.DeveloperRepository.AnyAsync(d => d.CodeName == codeName);
            } while (isExistDevCode);
            return codeName;
        }
    }
}
