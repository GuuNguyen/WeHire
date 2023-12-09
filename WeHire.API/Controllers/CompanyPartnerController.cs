using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.ComapnyPartnerServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyPartnerController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyPartnerController(ICompanyService companyService)
        {
            _companyService = companyService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetCompanyDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompanyAsync([FromQuery] PagingQuery query, [FromQuery] SearchCompanyDTO searchKey)
        {
            var result = _companyService.GetCompany(query, searchKey);
            var total = searchKey.AreAllPropertiesNull() ? await _companyService.GetTotalItemAsync() : result.Count;
            var paging = new PaginationInfo
            {
                Size = query.PageIndex,
                Page = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetCompanyDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result,
                Paging = paging
            });
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetCompanyDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompanyByIdAsync(int id)
        {
            var result = await _companyService.GetCompanyById(id);

            return Ok(new ApiResponse<GetCompanyDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result,
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetCompanyDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCompanyAsync([FromForm] CreateCompanyDTO requestBody)
        {
            var result = await _companyService.CreateCompanyAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetCompanyDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result,
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<GetCompanyDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCompanyAsync(int companyId,[FromForm] UpdateCompanyDTO requestBody)
        {
            var result = await _companyService.UpdateCompanyAsync(companyId, requestBody);

            return Created(string.Empty, new ApiResponse<GetCompanyDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result,
            });
        }

        [HttpDelete("{companyId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteCompanyAsync(int companyId)
        {
            await _companyService.DeleteCompanyAsync(companyId);

            return NoContent();
        }
    }
}
