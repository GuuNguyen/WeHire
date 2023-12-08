using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Application.DTOs.Payment
{
    public class CreatePaymentDTO
    {
        [Required(ErrorMessage = "PayPeriodId is required")]
        public int PayPeriodId { get; set; }
        [Required(ErrorMessage = "PayerId is required")]
        public int PayerId { get; set; }

        public string Description { get; set; }

        [DefaultValue("http://example.com/your_redirect_url.html")]
        public string ReturnUrl { get; set; } 

    }
}
