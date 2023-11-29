using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Education;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.EducationServices
{
    public interface IEducationService
    {
        public List<GetEducationByAdmin> GetEducationsByAdmin(PagingQuery query);
        public Task<List<GetEducationDTO>> GetEducationsByDevIdAsync(int developerId);
        public Task<GetEducationDTO> CreateEducationAsync(CreateEducationDTO requestBody);
        public Task<GetEducationDTO> UpdateEducationAsync(int educationId, UpdateEducationModel requestBody);
        public Task DeleteEducationAsync(int educationId);
    }
}
