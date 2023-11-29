using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.JobPosition;

namespace WeHire.Infrastructure.Services.JobPositionServices
{
    public interface IJobPositionService
    {
        Task<List<GetJobPosition>> GetJobPositionByProjectId(int projectId);
        Task<List<GetJpRequestModel>> GetJpWithHiringRequest(int projectId);
        Task<GetJobPosition> CreateJobPosition(CreateJobPosition requestBody);
        Task<GetJobPosition> UpdateJobPosition(int jobPositionId, UpdateJobPosition requestBody);
        Task<GetJobPosition> DeleteJobPosition(int jobPositionId);
    }
}
