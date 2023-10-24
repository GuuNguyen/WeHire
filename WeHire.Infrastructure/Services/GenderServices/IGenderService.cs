using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Gender;

namespace WeHire.Infrastructure.Services.GenderServices
{
    public interface IGenderService
    {
        public List<GetGenderDTO> GetAllGender();
    }
}
