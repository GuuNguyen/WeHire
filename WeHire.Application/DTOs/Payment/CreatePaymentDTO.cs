using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Payment
{
    public class CreatePaymentDTO
    {
        public int AgreementId { get; set; }
        public int PayerId { get; set; }
        public string Description { get; set; }

        [DefaultValue("http://example.com/your_redirect_url.html")]
        public string ReturnUrl { get; set; } 

    }
}
