using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Payment;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Application.Services.TransactionServices;
using static System.Net.WebRequestMethods;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.PaymentEnum;
using static WeHire.Domain.Enums.PayPeriodEnum;
using static WeHire.Domain.Enums.TransactionEnum;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionService _transactionService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private string _clientId, _secret, _baseUrl;

        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, ITransactionService transactionService)
        {
            _transactionService = transactionService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _clientId = _configuration["PayPal:ClientId"];
            _secret = _configuration["PayPal:Secret"];
            _baseUrl = _configuration["PayPal:BaseUrl"];
        }

        public static HttpClient GetPaypalHttpClient()
        {
            const string sandbox = "https://api.sandbox.paypal.com";

            var http = new HttpClient
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromMinutes(15),
            };

            return http;
        }

        public async Task<PayPalAccessToken> GetAccessTokenAsync()
        {
            var http = GetPaypalHttpClient();
            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes($"{_clientId}:{_secret}");

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            //var form = new Dictionary<string, string>
            //{
            //    ["grant_type"] = "client_credentials"
            //};
            //request.Content = new FormUrlEncodedContent(form);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
            request.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

            HttpResponseMessage response = await http.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
            return accessToken;
        }

        public async Task<string> CreatePayPalPaymentAsync(CreatePaymentDTO requestBody)
        {
            HttpResponseMessage response;
            var currencyStr = "USD";
            var paymentMethod = "paypal";
            var http = GetPaypalHttpClient();
            var accessToken = await GetAccessTokenAsync();
            var payPeriod = await _unitOfWork.PayPeriodRepository.Get(p => p.PayPeriodId == requestBody.PayPeriodId &&
                                                                           p.Status != (int)PayPeriodStatus.Paid)
                                                                 .SingleOrDefaultAsync();
            if(payPeriod == null)
               throw new ExceptionResponse(HttpStatusCode.BadRequest, "PayPeriod", "PayPeriod does not exist!");

            var amount = payPeriod.TotalAmount + payPeriod.TotalAmount * 8/100;

            var newTransaction = new Transaction
            {
                PayerId = requestBody.PayerId,
                PayPeriodId = requestBody.PayPeriodId,
                PayPalTransactionId = null,
                PaymentMethod = "PayPal",
                Amount = Math.Round((decimal)amount!, 0),
                Currency = currencyStr,
                Description = requestBody.Description,
                Timestamp = DateTime.Now,
                State = null,
                Status = (int)TransactionStatus.Failed
            };

            await _transactionService.CreateTransactionAsync(newTransaction);

            var payment = JObject.FromObject(new
            {
                intent = "CAPTURE",
                payment_source = new
                {
                    paypal = new
                    {
                        experience_context = new
                        {
                            payment_method_preference = "IMMEDIATE_PAYMENT_REQUIRED",
                            brand_name = "WeHire CO",
                            locale = "en-US",
                            landing_page = "LOGIN",
                            user_action = "PAY_NOW",
                            return_url = requestBody.ReturnUrl,
                            cancel_url = "https://example.com/cancelUrl"
                        }
                    }
                },
                purchase_units = JArray.FromObject(new[]
                {
                  new
                  {
                      amount = new
                      {
                          currency_code = "USD",
                          value = Math.Round((decimal)amount / 24240, 0)
                      }
                  }
              })
            });

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v2/checkout/orders");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
               throw new ExceptionResponse(HttpStatusCode.BadRequest, "PayPal", "Create payment failed!");

            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentCreatedResponse paypalPaymentCreated = JsonConvert.DeserializeObject<PayPalPaymentCreatedResponse>(content);
            string approvalUrl = paypalPaymentCreated.links.FirstOrDefault(link => link.rel == "payer-action")?.href;

            newTransaction.Status = (int)TransactionStatus.Created;
            newTransaction.PayPalTransactionId = paypalPaymentCreated.id;
            newTransaction.State = paypalPaymentCreated.state;
            _unitOfWork.TransactionRepository.Update(newTransaction);
            await _unitOfWork.SaveChangesAsync();

            return approvalUrl;
        }

        public async Task<PayPalPaymentExecutedResponse> ExecutePayPalPaymentAsync(string paymentId, string payerId)
        {
            var transaction = await _unitOfWork.TransactionRepository.Get(t => t.PayPalTransactionId == paymentId &&
                                                                               t.Status == (int)TransactionStatus.Created)
                                                                     .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Transaction", "Transaction does not exist!!");

            var http = GetPaypalHttpClient();
            var accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _baseUrl + $"/v2/checkout/orders/{paymentId}/capture");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            {
                payer_id = payerId
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentExecutedResponse executedPayment = JsonConvert.DeserializeObject<PayPalPaymentExecutedResponse>(content);     

            transaction.State = executedPayment.state;
            if (response.IsSuccessStatusCode)
            {
                transaction.Status = (int)TransactionStatus.Success;
                var payPeriod = await _unitOfWork.PayPeriodRepository.GetByIdAsync(transaction.PayPeriodId);
                payPeriod.Status = (int)PayPeriodStatus.Paid;
            }
            await _unitOfWork.SaveChangesAsync();

            return executedPayment;
        }
    }
}
