using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;

namespace WeHire.Application.DTOs.Developer
{
    public class ChangeStatusDTO
    {
        public int DeveloperId { get; set; }    
        public DeveloperStatus DevStatus { get; set; } 
    }
}
