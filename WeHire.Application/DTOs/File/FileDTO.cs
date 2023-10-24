using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.File
{
    public class FileDTO    
    {
        public IFormFile? File { get; set; }
    }
}
