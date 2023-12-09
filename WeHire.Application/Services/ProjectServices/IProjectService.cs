using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.ProjectServices
{
    public interface IProjectService
    {
        public List<GetListProjectDTO> GetAllProject(PagingQuery query, string? searchKeyString, SearchProjectDTO searchKey);
        public List<GetListProjectDTO> GetProjectByDevId(int devId, int devStatusInProject, string? searchKeyString, SearchProjectDTO searchKey);
        public List<GetListProjectDTO> GetAllProjectByCompanyId(int companyId, PagingQuery query, string? searchKeyString, SearchProjectDTO searchKey);
        public Task<GetProjectDetail> GetProjectById(int projectId);
        public Task<GetProjectDTO> CreateProjectAsync(CreateProjectDTO requestBody);
        public Task<GetProjectDTO> UpdateProjectAsync(int projectId, UpdateProjectDTO requestBody);
        public Task UpdateImageAsync(int projectId, IFormFile file);
        public Task<int> GetTotalProjectAsync(int? companyId = null);
    }
}
