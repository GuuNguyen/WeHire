﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.HiringRequestServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

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

       
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetAllFieldRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllHiringRequestAsync([FromQuery] PagingQuery query,
                                                                  [FromQuery] SearchHiringRequestDTO searchKey,
                                                                  [FromQuery] SearchExtensionDTO searchExtensionKey)
        {
            var result = _requestService.GetAllRequest(query, searchKey, searchExtensionKey);
            var total = (searchKey.AreAllPropertiesNull() && searchExtensionKey.AreAllPropertiesNull())
                        ? await _requestService.GetTotalRequestsAsync()
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
                Data = result
            });
        }

        [HttpGet("{requestId}")]
        [ProducesResponseType(typeof(ApiResponse<GetAllFieldRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestByDevId(int requestId)
        {
            var result = await _requestService.GetRequestByIdAsync(requestId);

            return Ok(new ApiResponse<GetAllFieldRequest>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("ByDev")]
        [ProducesResponseType(typeof(ApiResponse<List<GetAllFieldRequest>>), StatusCodes.Status200OK)]
        public IActionResult GetRequestByDevId([FromQuery] int devId, [FromQuery] int status)
        {
            var result =  _requestService.GetRequestsByDevId(devId, status);

            return Ok(new ApiResponse<List<GetAllFieldRequest>>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpGet("ByCompany")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetAllFieldRequest>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequestByCompanyId([FromQuery] int companyId,
                                                               [FromQuery] PagingQuery query,
                                                               [FromQuery] SearchHiringRequestDTO searchKey,
                                                               [FromQuery] SearchExtensionDTO searchExtensionKey)
        {
            var result = await _requestService.GetRequestsByCompanyId(companyId, query, searchKey, searchExtensionKey);
            var total = (searchKey.AreAllPropertiesNull() && searchExtensionKey.AreAllPropertiesNull())
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
                Data = result
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
    }
}
