using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.AssignTask;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.DeveloperTaskAssignment;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.AssignTaskServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignTaskController : ControllerBase
    {
        private readonly IAssignTaskService _assignTaskService;

        public AssignTaskController(IAssignTaskService assignTaskService)
        {
            _assignTaskService = assignTaskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAssignTaskDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCvAsync([FromQuery] PagingQuery query,
                                                       [FromQuery] SearchAssignTaskDTO searchKey)
        {
            var result = _assignTaskService.GetAllAssignTask(query, searchKey);
            var total = await _assignTaskService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetAssignTaskDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{taskId}")]
        [ProducesResponseType(typeof(ApiResponse<GetAssignTaskDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTaskById(int taskId)
        {
            var result = await _assignTaskService.GetAssignTaskByIdAsync(taskId);

            return Ok(new ApiResponse<GetAssignTaskDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("Status")]
        [ProducesResponseType(typeof(ApiResponse<List<EnumDetailDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetListStatusValue()
        {
            var result = _assignTaskService.GetTaskStatus();

            return Ok(new ApiResponse<List<EnumDetailDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetAssignTaskDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCvAsync(CreateAssignTaskDTO requestBody)
        {
            var result = await _assignTaskService.CreateAssignTaskAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetAssignTaskDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("Approval")]
        [ProducesResponseType(typeof(ApiResponse<GetAssignTaskDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ApprovalTask(ApprovalStatusTask requestBody)
        {
            var result = await _assignTaskService.ApprovalTaskAsync(requestBody);

            return Ok(new ApiResponse<GetAssignTaskDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpPut("Finished")]
        [ProducesResponseType(typeof(ApiResponse<GetAssignTaskDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FinishedTask(int taskId)
        {
            var result = await _assignTaskService.FinishTaskAsync(taskId);

            return Ok(new ApiResponse<GetAssignTaskDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPut("ChangeStatusDevTask")]
        [ProducesResponseType(typeof(ApiResponse<GetDevTaskAssignmentDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatusDevTask(ChangeStatusDevTaskAssignmentDTO requestBody)
        {
            var result = await _assignTaskService.ChangeStatusDevTaskAssignmentAsync(requestBody);

            return Ok(new ApiResponse<GetDevTaskAssignmentDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

    }
}
