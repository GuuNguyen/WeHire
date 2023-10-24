using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.ErrorHandler
{
    public class ExceptionResponse : Exception
    {
        public string? ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }
        public string? ErrorField { get; }

        public ExceptionResponse(HttpStatusCode statusCode, string errorField, string errorMessage)
        {          
            StatusCode = statusCode;
            ErrorField = errorField;
            ErrorMessage = errorMessage;
        }
    }
}
