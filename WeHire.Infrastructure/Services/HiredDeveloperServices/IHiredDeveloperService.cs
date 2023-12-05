using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiredDeveloper;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.HiredDeveloperServices
{
    public interface IHiredDeveloperService
    {
        public Task<List<GetMatchingDev>> GetDevsInHiringRequest(int requestId);
        public Task<List<GetHiredDeveloperModel>> SendDevToHR(SendDevDTO requestBody);
        public Task<GetHiredDeveloperModel> RejectDeveloperAsync(int requestId, int developerId);
        public Task<GetHiredDeveloperModel> TerminateDeveloperAsync(int projectId, int developerId);
    }
}
