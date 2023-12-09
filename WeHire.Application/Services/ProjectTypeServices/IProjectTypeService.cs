using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProjectType;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.ProjectTypeServices
{
    public interface IProjectTypeService
    {
        public List<GetProjectTypeDTO> GetAllProjectTypeAsync(PagingQuery query, SearchProjectType searchKey);
        public Task<GetProjectTypeDTO> CreateProjectTypeAsync(string projectTypeName);
        public Task<GetProjectTypeDTO> UpdateProjectTypeAsync(int projectTypeId, UpdateProjectTypeDTO requestBody);
        public Task DeleteProjectTypeAsync(int projectTypeId);
        public Task<int> GetTotalItemAsync();
    }
}
