using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.DTOs.User;
using WeHire.Infrastructure.Services.ComapnyPartnerServices;
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
        [ProducesResponseType(typeof(ApiResponse<GetCompanyDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UpdateCompanyAsync(int companyId,[FromForm] UpdateCompanyDTO requestBody)
        {
            var result = await _companyService.UpdateCompanyAsync(companyId, requestBody);

            return Created(string.Empty, new ApiResponse<GetCompanyDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result,
            });
        }
    }
}
