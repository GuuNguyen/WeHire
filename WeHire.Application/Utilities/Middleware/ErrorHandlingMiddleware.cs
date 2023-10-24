using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeHire.Application.Utilities.ErrorHandler;

namespace WeHire.Application.Utilities.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ExceptionResponse ex)
            {
                string? errorField = ex.ErrorField;
                string? errorMessage = ex.ErrorMessage;
                HttpStatusCode statusCode = ex.StatusCode;

                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentType = "application/json";

                var error = new Error
                {
                    code = (int)statusCode,
                    field = errorField,
                    message = errorMessage
                };
                string jsonResponse = JsonSerializer.Serialize(error);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

}
