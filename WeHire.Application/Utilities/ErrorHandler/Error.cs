using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.Utilities.ErrorHandler
{
    public class Error
    {
        public int code { get; set; }
        public string? field { get; set; }
        public string? message { get; set; }
    }
}
