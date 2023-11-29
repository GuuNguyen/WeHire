using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class CreatePayPeriodModel
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public DateTime InputDate { get; set; }
    }
}
