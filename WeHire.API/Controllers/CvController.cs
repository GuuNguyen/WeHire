using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.DTOs.CV;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.CvServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvController : ControllerBase
    {
        private readonly ICvService _cvService;

        public CvController(ICvService cvService)
        {
            _cvService = cvService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetCvDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCvAsync([FromQuery] PagingQuery query, 
                                                  [FromQuery] SearchCvDTO searchKey)
        {
            var result = _cvService.GetAllCv(query, searchKey);
            var total = await _cvService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetCvDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetCvDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCvByIdAsync(int id)
        {
            var result = await _cvService.GetCvByIdAsync(id);
            return Ok(new ApiResponse<GetCvDetail>()
            {
                Code= StatusCodes.Status200OK,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetCvDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCvAsync(CreateCvDTO requestBody)
        {
            var result = await _cvService.CreateCvAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetCvDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }
    }
}
