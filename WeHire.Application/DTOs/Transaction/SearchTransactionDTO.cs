using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Transaction
{
    public class SearchTransactionDTO
    {
        public string? PayPalTransactionId { get; set; }
        public decimal? Amount { get; set; }
        public int? Status { get; set; }
    }
}
