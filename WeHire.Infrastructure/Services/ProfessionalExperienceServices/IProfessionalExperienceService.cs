using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.ProfessionalExperience;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.ProfessionalExperienceServices
{
    public interface IProfessionalExperienceService
    {
        public Task<List<GetProfessionalExperience>> GetProfessionalExperiencesByDevIdAsync(int developerId);
        public List<GetPEByAdmin> GetProfessionalExperiencesAsync(PagingQuery query);
        public Task<GetProfessionalExperience> CreateProfessionalExperienceAsync(CreateProfessionalExperience requestBody);
        public Task<GetProfessionalExperience> UpdateProfessionalExperienceAsync(int professionalExperienceId, UpdateProfessionalExperience requestBody);
        public Task DeleteProfessionalExperienceAsync(int professionalExperienceId);
    }
}
