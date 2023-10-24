using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeHire.Application.DTOs.CV
{
    public class GetCvDetail
    {
        public int CvId { get; set; }
        public string CvCode { get; set; }
        public string Src { get; set; }
        public string DevFullName { get; set; }
    }
}
