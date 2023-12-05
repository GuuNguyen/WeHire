﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.ProjectServices;
using WeHire.Application.DTOs.Project;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.DTOs.File;

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
        public async Task<IActionResult> GetListProjectAsync([FromQuery] PagingQuery query,
                                      [FromQuery] string? searchKeyString, [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProject(query, searchKeyString, searchKey);
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
                Data = result
            });
        }

        [HttpGet("ByCompany/{companyId}")]
        [ProducesResponseType(typeof(PagedApiResponse<GetListProjectDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListProjectAsync(int companyId, 
                                                            [FromQuery] PagingQuery query,
                                                            [FromQuery] string? searchKeyString,
                                                            [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetAllProjectByCompanyId(companyId, query, searchKeyString, searchKey);
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
                Data = result
            });
        }

        [HttpGet("Developer/{developerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetListProjectDTO>>), StatusCodes.Status200OK)]
        public  IActionResult GetProjectByDeveloperIdt(int developerId, [FromQuery] string? searchKeyString, [FromQuery] int devStatusInProject, [FromQuery] SearchProjectDTO searchKey)
        {
            var result = _projectService.GetProjectByDevId(developerId, devStatusInProject, searchKeyString, searchKey);
            return Ok(new ApiResponse<List<GetListProjectDTO>>()
            {
                Code = StatusCodes.Status200OK,
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
