﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.Type;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.TypeServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private readonly ITypeService _typeService;

        public TypeController(ITypeService typeService)
        {
            _typeService = typeService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetTypeDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTypeAsync([FromQuery] PagingQuery query, 
                                                         [FromQuery] SearchTypeDTO searchKey)
        {
            var result = _typeService.GetAllType(query, searchKey);
            var total = await _typeService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetTypeDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetTypeDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTypeAsync(CreateTypeDTO requestBody)
        {
            var result = await _typeService.CreateTypeAsync(requestBody);
            return Created(string.Empty, new ApiResponse<GetTypeDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTypeAsync(UpdateTypeModel requestBody)
        {
            await _typeService.UpdateTypeAsync(requestBody);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Update success",
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{typeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTypeAsync(int typeId)
        {
            await _typeService.DeleteTypeAsync(typeId);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Delete success",
            });
        }
    }
}
