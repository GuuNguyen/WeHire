﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using WeHire.Entity.IRepositories;
using WeHire.Entity.Repositories;
using WeHire.Infrastructure.Services.AgreementServices;
using WeHire.Infrastructure.Services.TransactionServices;
using static System.Net.WebRequestMethods;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.PaymentEnum;

namespace WeHire.Infrastructure.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionService _transactionService;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private string _clientId, _secret;
        public PaymentService(IConfiguration configuration, IUnitOfWork unitOfWork, ITransactionService transactionService)
        {
            _transactionService = transactionService;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _clientId = _configuration["PayPal:ClientId"];
            _secret = _configuration["PayPal:Secret"];
        }

        public static HttpClient GetPaypalHttpClient()
        {
            const string sandbox = "https://api.sandbox.paypal.com";

            var http = new HttpClient
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromSeconds(30),
            };

            return http;
        }

        public async Task<PayPalAccessToken> GetAccessTokenAsync()
        {
            var http = GetPaypalHttpClient();
            byte[] bytes = Encoding.GetEncoding("iso-8859-1").GetBytes($"{_clientId}:{_secret}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            };
            request.Content = new FormUrlEncodedContent(form);

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
            var agreement = await _unitOfWork.AgreementRepository.GetByIdAsync(requestBody.AgreementId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.AGREEMENT_FIELD, ErrorMessage.AGREEMENT_NOT_EXIST);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var newTransaction = new Transaction
            {
                PayerId = requestBody.PayerId,
                AgreementId = requestBody.AgreementId,
                PayPalTransactionId = null,
                PaymentMethod = paymentMethod,
                Amount = (int)agreement.TotalCommission,
                Currency = currencyStr,
                Description = requestBody.Description,
                Timestamp = DateTime.Now,
                State = null,
                Status = (int)TransactionEnum.TransactionStatus.Failed
            };
            await _transactionService.CreateTransactionAsync(newTransaction);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                var payment = JObject.FromObject(new
                {
                    intent = "sale",
                    redirect_urls = new
                    {
                        return_url = requestBody.ReturnUrl,
                        cancel_url = "http://example.com/your_cancel_url.html"
                    },
                    payer = new { payment_method = paymentMethod },
                    transactions = JArray.FromObject(new[]
                    {
                        new
                        {
                            amount = new
                            {
                                total = (double)agreement.TotalCommission,
                                currency = currencyStr
                            },
                            description = requestBody.Description,
                        }
                    }),
                    application_context = new { shipping_preference = "NO_SHIPPING" }
                });

                request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

                response = await http.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception();
                transaction.Commit();   
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            
            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentCreatedResponse paypalPaymentCreated = JsonConvert.DeserializeObject<PayPalPaymentCreatedResponse>(content);
            string approvalUrl = paypalPaymentCreated.links.FirstOrDefault(link => link.rel == "approval_url")?.href;

            var _UnitOfWork_2 = new UnitOfWork(new WeHireDBContext { });
            using var transaction_2 = _UnitOfWork_2.BeginTransaction();
            newTransaction.Status = (int)TransactionEnum.TransactionStatus.Created;
            newTransaction.PayPalTransactionId = paypalPaymentCreated.id;
            newTransaction.State = paypalPaymentCreated.state;
            _UnitOfWork_2.TransactionRepository.Update(newTransaction);
            await _UnitOfWork_2.SaveChangesAsync();
            transaction_2.Commit();

            return approvalUrl;
        }

        public async Task<PayPalPaymentExecutedResponse> ExecutePayPalPaymentAsync(string paymentId, string payerId)
        {
            var http = GetPaypalHttpClient();
            var accessToken = await GetAccessTokenAsync();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment/{paymentId}/execute");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            { 
                payer_id = payerId
            });

            request.Content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await http.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();
            PayPalPaymentExecutedResponse executedPayment = JsonConvert.DeserializeObject<PayPalPaymentExecutedResponse>(content);

            var transaction = await _unitOfWork.TransactionRepository.Get(t => t.PayPalTransactionId == paymentId).SingleOrDefaultAsync();
            transaction.State = executedPayment.state;
            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return executedPayment;
        }     
    }
}
