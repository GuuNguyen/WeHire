using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Payment;

namespace WeHire.Application.Services.PaymentServices
{
    public interface IPaymentService
    {
        Task<string> CreatePayPalPaymentAsync(CreatePaymentDTO requestBody);
        Task<PayPalPaymentExecutedResponse> ExecutePayPalPaymentAsync(string paymentId, string payerId);
    }
}
