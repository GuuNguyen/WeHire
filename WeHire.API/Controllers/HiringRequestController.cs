using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.HiringRequestServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiringRequestController : ControllerBase
    {
        private readonly IHiringRequestService _requestService;

        public HiringRequestController(IHiringRequestService requestService)
        {
            _requestService = requestService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetListHiringRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllHiringRequestAsync([FromQuery] PagingQuery query,
                                                                  [FromQuery] SearchHiringRequestDTO searchKey,
                                                                  [FromQuery] SearchExtensionDTO searchExtensionKey)
        {
            var result = _requestService.GetAllRequest(searchKey, searchExtensionKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = (searchKey.AreAllPropertiesNull() && searchExtensionKey.AreAllPropertiesNull())
                        ? await _requestService.GetTotalRequestsAsync()
                        : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };

            return Ok(new PagedApiResponse<GetListHiringRequest>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<GetAllFieldRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestById(int requestId)
        {
            var result = await _requestService.GetRequestByIdAsync(requestId);

            return Ok(new ApiResponse<GetAllFieldRequest>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("ByDev")]
        [ProducesResponseType(typeof(ApiResponse<List<GetAllFieldRequest>>), StatusCodes.Status200OK)]
        public IActionResult GetRequestByDevId([FromQuery] int devId, 
                                               [FromQuery] string? searchKeyString, 
                                               [FromQuery] SearchHiringRequestDTO searchKey)
        {
            var result =  _requestService.GetRequestsByDevId(devId, searchKeyString, searchKey);

            return Ok(new ApiResponse<List<GetAllFieldRequest>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("ByCompany")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldRequest>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestByCompanyId([FromQuery] int companyId,
                                                               [FromQuery] PagingQuery query,
                                                               [FromQuery] string? searchKeyString,
                                                               [FromQuery] SearchHiringRequestDTO searchKey,
                                                               [FromQuery] SearchExtensionDTO searchExtensionKey)
        {
            var result = await _requestService.GetRequestsByCompanyId(companyId, searchKeyString, searchKey, searchExtensionKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = (searchKey.AreAllPropertiesNull() && searchExtensionKey.AreAllPropertiesNull() && searchKeyString == null)
                       ? await _requestService.GetTotalRequestsAsync(companyId)
                       : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetAllFieldRequest>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("ByProject")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldRequest>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestByProject([FromQuery] int projectId,
                                                             [FromQuery] PagingQuery query,
                                                             [FromQuery] string? searchKeyString,
                                                             [FromQuery] SearchHiringRequestDTO searchKey,
                                                             [FromQuery] SearchExtensionDTO searchExtensionKey)
        {
            var result = await _requestService.GetRequestsByProjectId(projectId, searchKeyString, searchKey, searchExtensionKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = (searchKey.AreAllPropertiesNull() && searchExtensionKey.AreAllPropertiesNull() && searchKeyString == null)
                       ? await _requestService.GetTotalRequestsByProjectIdAsync(projectId)
                       : result.Count;

            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetAllFieldRequest>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }


        [HttpGet("Status")]
        [ProducesResponseType(typeof(ApiResponse<List<EnumDetailDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetRequestStatus()
        {
            var result = _requestService.GetRequestStatus();

            return Ok(new ApiResponse<List<EnumDetailDTO>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateRequestAsync(CreateRequestDTO requestBody)
        {
            var result = await _requestService.CreateRequestAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [Authorize]
        [HttpPost("Clone/{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CloneARequest(int requestId)
        {
            var result = await _requestService.CloneARequestAsync(requestId);

            return Created(string.Empty, new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }


        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<GetRequestDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UpdateRequestAsync(int requestId, UpdateRequestDTO requestBody)
        {
            var result = await _requestService.UpdateRequestAsync(requestId, requestBody);

            return Created(string.Empty, new ApiResponse<GetRequestDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpDelete("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var result = await _requestService.DeleteHiringRequest(requestId);

            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
