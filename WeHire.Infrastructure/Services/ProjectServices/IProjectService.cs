using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.ProjectServices
{
    public interface IProjectService
    {
        public List<GetListProjectDTO> GetAllProject(PagingQuery query, SearchProjectDTO searchKey);
        public List<GetListProjectDTO> GetAllProjectByCompanyId(int companyId, PagingQuery query, SearchProjectDTO searchKey);
        public Task<GetProjectDetail> GetProjectById(int projectId);
        public Task<GetProjectDTO> CreateProjectAsync(CreateProjectDTO requestBody);
        public Task<GetProjectDTO> UpdateProjectAsync(int projectId, UpdateProjectDTO requestBody);
        public Task<int> GetTotalProjectAsync(int? companyId = null);
    }
}
