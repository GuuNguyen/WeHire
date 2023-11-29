using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Infrastructure.Services.PaymentServices;
using WeHire.Infrastructure.Services.PaySlipServices;
using WeHire.Application.DTOs.Level;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.DTOs.PaySlip;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaySlipController : ControllerBase
    {
        private readonly IPaySlipService _paySlipService;

        public PaySlipController(IPaySlipService paySlipService)
        {
            _paySlipService = paySlipService;
        }

        [HttpGet("ByPayPeriod/{payPeriodId}")]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetPaySlipModel>>), StatusCodes.Status200OK)]
        public IActionResult GetAllPaySlip(int payPeriodId, [FromQuery] PagingQuery query)
        {
            var result = _paySlipService.GetPaySlipsByPayPeriodId(payPeriodId, query);
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = result.Count
            };

            return Ok(new PagedApiResponse<GetPaySlipModel>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<GetUpdatePaySlipResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePaySlip(UpdatePaySlipModel requestBody)
        {
            var result = await _paySlipService.UpdateTotalOvertimeHourAsync(requestBody);

            return Ok(new ApiResponse<GetUpdatePaySlipResponse>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
