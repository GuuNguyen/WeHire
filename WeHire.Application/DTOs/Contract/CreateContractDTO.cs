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
        [Required(ErrorMessage = "RequestId is required")]
        public int RequestId { get; set; }
        [Required(ErrorMessage = "DeveloperId is required")]
        public int DeveloperId { get; set; }
        [Required(ErrorMessage = "FromDate is required")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "ToDate is required")]
        public DateTime? ToDate { get; set; }
        [Required(ErrorMessage = "LegalRepresentation is required")]
        public string LegalRepresentation { get; set; }
        [Required(ErrorMessage = "LegalRepresentationPosition is required")]
        public string LegalRepresentationPosition { get; set; }
    }
}
