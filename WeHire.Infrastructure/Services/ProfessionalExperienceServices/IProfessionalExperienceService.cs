using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProfessionalExperience;

namespace WeHire.Infrastructure.Services.ProfessionalExperienceServices
{
    public interface IProfessionalExperienceService
    {
        public Task<List<GetProfessionalExperience>> GetProfessionalExperiencesByDevIdAsync(int developerId);
        public Task<GetProfessionalExperience> CreateProfessionalExperienceAsync(CreateProfessionalExperience requestBody);
    }
}
