using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.File;

namespace WeHire.Application.DTOs.CV
{
    public class CreateCvDTO : FileDTO
    {
        public string Src { get; set; }
        public int? DeveloperId { get; set; }
    }
}
