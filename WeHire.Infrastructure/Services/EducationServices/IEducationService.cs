using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Education;

namespace WeHire.Infrastructure.Services.EducationServices
{
    public interface IEducationService
    {
        public Task<List<GetEducationDTO>> GetEducationsByDevIdAsync(int developerId);
        public Task<GetEducationDTO> CreateEducationAsync(CreateEducationDTO requestBody);
    }
}
