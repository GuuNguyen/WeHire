using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.ProjectServices;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.CheckNullProperties;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(PagedApiResponse<GetListProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectAsync([FromQuery] PagingQuery query, [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProject(query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _projectService.GetTotalProjectAsync()
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetListProjectDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("ByCompany/{companyId}")]
        [ProducesResponseType(typeof(PagedApiResponse<GetListProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectAsync(int companyId, 
                                                            [FromQuery] PagingQuery query,
                                                            [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProjectByCompanyId(companyId, query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _projectService.GetTotalProjectAsync(companyId)
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetListProjectDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectAsync(int projectId)
        {
            var result = await _projectService.GetProjectById(projectId);
            return Ok(new ApiResponse<GetProjectDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProjectAsync([FromForm] CreateProjectDTO requestBody)
        {
            var result = await _projectService.CreateProjectAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetProjectDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProjectAsync(int projectId, [FromForm] UpdateProjectDTO requestBody)
        {
            var result = await _projectService.UpdateProjectAsync(projectId, requestBody);

            return Ok(new ApiResponse<GetProjectDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
