using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.PayPeriod
{
    public class ExcelFileModel
    {
        public string FileName { get; set; }
        public byte[] ExcelByteArray { get; set; }
    }
}
