using System;
using System.Collections.Generic;

#nullable disable

namespace WeHire.Domain.Entities
{
    public partial class Cv
    {
        public int CvId { get; set; }
        public string CvCode { get; set; }
        public string Src { get; set; }
        public bool? IsOwned { get; set; }
        public int? DeveloperId { get; set; }

        public virtual Developer Developer { get; set; }
    }
}
