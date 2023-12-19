using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.Services.ProjectServices;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.DTOs.File;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetListProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectAsync([FromQuery] PagingQuery query,
                                      [FromQuery] string? searchKeyString, [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProject(searchKeyString, searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() && searchKeyString == null ? await _projectService.GetTotalProjectAsync()
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
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("ByCompany/{companyId}")]
        [ProducesResponseType(typeof(PagedApiResponse<GetListProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectByCompanyAsync(int companyId, 
                                                            [FromQuery] PagingQuery query,
                                                            [FromQuery] string? searchKeyString,
                                                            [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProjectByCompanyId(companyId, searchKeyString, searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() && searchKeyString == null ? await _projectService.GetTotalProjectAsync(companyId)
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
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("Developer/{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetListProjectDTO>>), StatusCodes.Status200OK)]
        public  IActionResult GetProjectByDeveloperIdt(int developerId, [FromQuery] string? searchKeyString, [FromQuery] List<int> devStatusInProject, [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetProjectByDevId(developerId, devStatusInProject, searchKeyString, searchKey);
            return Ok(new ApiResponse<List<GetListProjectDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProjectAsync(CreateProjectDTO requestBody)
        {
            var result = await _projectService.CreateProjectAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetProjectDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [Authorize]
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

        [Authorize]
        [HttpPut("CloseByHR/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CloseProjectByHRAsync(int projectId)
        {
            var result = await _projectService.CloseProjectByHRAsync(projectId);

            return Ok(new ApiResponse<GetProjectDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPut("CloseByManager/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CloseProjectByManagerAsync(int projectId)
        {
            var result = await _projectService.CloseProjectByManagerAsync(projectId);

            return Ok(new ApiResponse<GetProjectDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpPut("UpdateImage/{projectId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateImageAsync(int projectId, [FromForm] FileDTO file)
        {
            if (file == null || file.File == null || file.File.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileExtension = Path.GetExtension(file.File.FileName).ToLowerInvariant();
            if (fileExtension != ".jpg" && fileExtension != ".png" && fileExtension != ".jpeg")
            {
                return BadRequest("Please upload a valid Excel file.");
            }

            await _projectService.UpdateImageAsync(projectId, file.File);

            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Update image successful!"
            });
        }
    }
}
