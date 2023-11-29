using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.Contract
{
    public class CreateContractDTO
    {
        [Required]
        public int RequestId { get; set; }
        [Required]
        public int DeveloperId { get; set; }
        [Required]
        public DateTime FromDate { get; set; }
        [Required]
        public DateTime? ToDate { get; set; }
        [Required]
        public string LegalRepresentation { get; set; }
        [Required]
        public string LegalRepresentationPosition { get; set; }
    }
}
