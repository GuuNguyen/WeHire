using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
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

        public List<GetListProjectDTO> GetAllProject(PagingQuery query, string? searchKeyString, SearchProjectDTO searchKey)
        {
            IQueryable<Project> projects = _unitOfWork.ProjectRepository.GetAll()
                                                        .Include(p => p.Company)
                                                        .Include(p => p.ProjectType)
                                                        .OrderByDescending(n => n.CreatedAt);

            projects = SearchProjectByString(projects, searchKeyString);
            projects = projects.SearchItems(searchKey);
            projects = projects.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedProjects = _mapper.Map<List<GetListProjectDTO>>(projects);
            return mappedProjects;
        }

        public List<GetListProjectDTO> GetAllProjectByCompanyId(int companyId, PagingQuery query, string? searchKeyString, SearchProjectDTO searchKey)
        {
            IQueryable<Project> projects = _unitOfWork.ProjectRepository.Get(p => p.CompanyId == companyId)
                                                      .Include(p => p.Company)
                                                      .Include(p => p.ProjectType)
                                                      .OrderByDescending(n => n.CreatedAt);
            
            projects = SearchProjectByString(projects, searchKeyString);
            projects = projects.SearchItems(searchKey);
            projects = projects.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedProjects = _mapper.Map<List<GetListProjectDTO>>(projects.ToList());
            return mappedProjects;
        }
        private IQueryable<Project> SearchProjectByString(IQueryable<Project> query, string? searchKeyString)
        {
            if(searchKeyString != null)
                return query = query.Where(p => p.ProjectCode.Contains(searchKeyString) || p.ProjectName.Contains(searchKeyString));
            return query;
        }

        public async Task<GetProjectDetail> GetProjectById(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId)
                                                      .Include(p => p.Company)
                                                      .Include(p => p.HiredDevelopers.Where(h => h.Status == (int)HiredDeveloperStatus.Working))
                                                      .ThenInclude(h => h.Contract)
                                                      .Include(p => p.ProjectType)
                                                      .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_EXIST);
            var mappedProject = _mapper.Map<GetProjectDetail>(project);

            mappedProject.MinStartDate = ConvertDateTime.ConvertDateToStringNumber(project.HiredDevelopers.Min(h => h.Contract.FromDate));
            mappedProject.MaxEndDate = ConvertDateTime.ConvertDateToStringNumber(project.HiredDevelopers.Max(h => h.Contract.ToDate));

            var dayLeft = CalculateRemainingDays(DateTime.Now, project.EndDate);
            mappedProject.DayLeft = dayLeft;
            mappedProject.DayLeftPercent = CalculatePercentageRemaining(dayLeft, project.EndDate, project.StartDate);
            return mappedProject;
        }

        public List<GetListProjectDTO> GetProjectByDevId(int devId, int devStatusInProject, string? searchKeyString, SearchProjectDTO searchKey)
        {
            var projects = _unitOfWork.HiredDeveloperRepository.Get(h => h.DeveloperId == devId &&
                                                                         h.Status == devStatusInProject).AsNoTracking()
                                                              .Include(h => h.Project)
                                                              .Include(h => h.Project.ProjectType)
                                                              .Include(h => h.Project.Company)
                                                              .Select(h => h.Project)
                                                              .AsQueryable();

            projects = SearchProjectByString(projects, searchKeyString);
            projects = projects.SearchItems(searchKey);

            var mappedProject = _mapper.Map<List<GetListProjectDTO>>(projects);
            return mappedProject;
        }

        public async Task<GetProjectDTO> CreateProjectAsync(CreateProjectDTO requestBody)
        {
            var isExistCompany = await _unitOfWork.CompanyRepository.AnyAsync(c => c.CompanyId == requestBody.CompanyId);

            if (!isExistCompany)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var newProject = _mapper.Map<Project>(requestBody);
            newProject.ProjectCode = await GenerateUniqueCodeName();
            newProject.CreatedAt = DateTime.Now;
            newProject.NumberOfDev = 0;

            newProject.Status = requestBody.StartDate <= DateTime.Now ? (int)ProjectStatus.InProcess 
                                                                      : (int)ProjectStatus.Preparing;
            await _unitOfWork.ProjectRepository.InsertAsync(newProject);
            await _unitOfWork.SaveChangesAsync();

            var mappedProject = _mapper.Map<GetProjectDTO>(newProject);
            return mappedProject;
        }

        public async Task<GetProjectDTO> UpdateProjectAsync(int projectId, UpdateProjectDTO requestBody)
        {
            if (projectId != requestBody.ProjectId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PROJECT_FIELD, ErrorMessage.PROJECT_NOT_MATCH);

            var project = await _unitOfWork.ProjectRepository.Get(p => p.ProjectId == projectId && 
                                                                       p.Status != (int)ProjectStatus.Closed)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            var updateProject = _mapper.Map(requestBody, project);
            updateProject.Status = requestBody.StartDate <= DateTime.Now ? (int)ProjectStatus.InProcess
                                                                         : (int)ProjectStatus.Preparing;
            updateProject.UpdatedAt = DateTime.Now;
            _unitOfWork.ProjectRepository.Update(updateProject);

            await _unitOfWork.SaveChangesAsync();

            var mappedProject = _mapper.Map<GetProjectDTO>(updateProject);
            return mappedProject;
        }

        public async Task UpdateImageAsync(int projectId, IFormFile file)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);
            project.BackgroundImage = await _fileService.UploadFileAsync(file, project.ProjectName, ChildFolderName.BACKGROUND_FOLDER);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalProjectAsync(int? companyId = null)
        {
            var total = 0;
            var query = _unitOfWork.ProjectRepository.GetAll();

            if (companyId.HasValue)
               return total = await query.Where(p => p.CompanyId == companyId).CountAsync();

            return total = await query.CountAsync();
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

        private int CalculateRemainingDays(DateTime currentDate, DateTime? endDate)
        {
            TimeSpan remainingTime = (TimeSpan)(endDate - currentDate);
            return remainingTime.Days;
        }

        private double CalculatePercentageRemaining(int dayLeft, DateTime? endDate, DateTime? startDate)
        {
            TimeSpan totalTimeSpan = (TimeSpan)(endDate - startDate);

            double totalDays = totalTimeSpan.Days;

            double percentageRemaining = Math.Round((dayLeft / totalDays) * 100, 0);
            return percentageRemaining;
        }
    }
}
