using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.Payment;
using WeHire.Infrastructure.Services.PaymentServices;
using static System.Net.WebRequestMethods;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("Create")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePayment(CreatePaymentDTO requestBody)
        {
            var approvalUrl = await _paymentService.CreatePayPalPaymentAsync(requestBody);

            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = approvalUrl
            });
        }

        [HttpPost("Execute")]
        public async Task<IActionResult> ExecutePayment(string paymentId, string payerId)
        {
            var payment = await _paymentService.ExecutePayPalPaymentAsync(paymentId, payerId);
            return Ok(payment);
        }

    }
}
