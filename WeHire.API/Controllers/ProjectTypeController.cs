using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Project;
using WeHire.Application.DTOs.ProjectType;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.ProjectTypeServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTypeController : ControllerBase
    {
        private readonly IProjectTypeService _projectTypeService;

        public ProjectTypeController(IProjectTypeService projectTypeService)
        {
            _projectTypeService = projectTypeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetProjectTypeDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProjectTypeAsync([FromQuery] PagingQuery query,
                                                                [FromQuery] SearchProjectType searchKey)
        {
            var result = _projectTypeService.GetAllProjectTypeAsync(query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _projectTypeService.GetTotalItemAsync() 
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetProjectTypeDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetProjectTypeDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProjectTypeAsync(string projectTypeName)
        {
            var result = await _projectTypeService.CreateProjectTypeAsync(projectTypeName);

            return Created(string.Empty, new ApiResponse<GetProjectTypeDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }


        [HttpPut("{projectTypeId}")]
        [ProducesResponseType(typeof(ApiResponse<GetProjectTypeDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProjectTypeAsync(int projectTypeId, UpdateProjectTypeDTO requestBody)
        {
            var result = await _projectTypeService.UpdateProjectTypeAsync(projectTypeId, requestBody);

            return Ok(new ApiResponse<GetProjectTypeDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpDelete("{projectTypeId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveProjectTypeAsync(int projectTypeId)
        {
            await _projectTypeService.DeleteProjectTypeAsync(projectTypeId);

            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Delete success!"
            });
        }
    }
}
