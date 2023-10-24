using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Agreement;
using WeHire.Application.DTOs.CV;
using WeHire.Infrastructure.Services.AgreementServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgreementController : ControllerBase
    {
        private readonly IAgreementService _agreementService;

        public AgreementController(IAgreementService agreementService)
        {
            _agreementService = agreementService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetAgreementDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCvAsync(int requestId)
        {
            var result = await _agreementService.CreateAgreementAsync(requestId);

            return Created(string.Empty, new ApiResponse<GetAgreementDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }
    }
}
