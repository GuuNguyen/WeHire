using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Interview;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.InterviewServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewService _interviewService;

        public InterviewController(IInterviewService interviewService)
        {
            _interviewService = interviewService;
        }

        [HttpGet("ByManager")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetInterviewDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInterviewByManager([FromQuery] PagingQuery query,
                                                                  int? companyId,
                                                                  [FromQuery] SearchInterviewWithRequest searchKey)
        {
            var result = _interviewService.GetInterviewsByManager(query, companyId, searchKey);

            var total = !companyId.HasValue && searchKey.AreAllPropertiesNull() ? await _interviewService.GetTotalInterviewsAsync()
                                                                                : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetInterviewDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("ByHR")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetInterviewDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInterviewByHR(int? companyId,
                                                            [FromQuery] int? requestId,
                                                            [FromQuery] PagingQuery query,
                                                            [FromQuery] SearchInterviewDTO searchKey)
        {
            var result = _interviewService.GetInterviewsByCompany(companyId, requestId, query, searchKey);

            var total = !requestId.HasValue && searchKey.AreAllPropertiesNull() ? await _interviewService.GetTotalInterviewsAsync(companyId) 
                                                                                : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetInterviewDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{interviewId}")]
        [ProducesResponseType(typeof(PagedApiResponseSpecificData<GetInterviewWithDev>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInterviewById(int interviewId,[FromQuery] PagingQuery query)
        {
            var result = await _interviewService.GetInterviewById(interviewId, query);
            var total = await _interviewService.GetTotalDevInterviewAsync(interviewId);

            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponseSpecificData<GetInterviewWithDev>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result 
            });
        }

        [HttpGet("Request/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetInterviewDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInterviewByRequestId(int requestId)
        {
            var result = await _interviewService.GetInterviewByRequestIdAsync(requestId);

            return Ok(new ApiResponse<List<GetInterviewDetail>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }


        [HttpGet("Dev/{devId}")]
        [ProducesResponseType(typeof(ApiResponse<List<GetInterviewDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInterviewByDevId(int devId)
        {
            var result = await _interviewService.GetInterviewByDevId(devId);

            return Ok(new ApiResponse<List<GetInterviewDetail>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetInterviewDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateInterviewAsync(CreateInterviewDTO requestBody)
        {
            var result = await _interviewService.CreateInterviewAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetInterviewDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut("ApprovalByManager")]
        [ProducesResponseType(typeof(ApiResponse<GetInterviewDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatus(ChangeStatusDTO requestBody)
        {
            var result = await _interviewService.ChangeStatusAsync(requestBody);

            return Ok(new ApiResponse<GetInterviewDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
