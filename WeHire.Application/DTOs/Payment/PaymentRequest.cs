using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Payment
{
    public class PaymentRequest
    {
        public int Amount { get; set; } 
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
