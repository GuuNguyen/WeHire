using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;

namespace WeHire.Application.Services.DeveloperServices
{
    public interface IDeveloperService
    {
        public List<GetAllFieldDev> GetAllDev(PagingQuery query, SearchDeveloperDTO searchKey);
        public Task<GetDevDetail> GetDevByIdAsync(int devId);
        public List<GetDeveloperInProject> GetDevsByProjectId(DevInProjectRequestModel requestBody);
        public Task<List<GetMatchingDev>> GetDevMatchingWithRequest(int requestId);
        public Task<GetDevDTO> CreateDevAsync(CreateDevDTO requestBody);
        public Task<GetDevDTO> UpdateDevProfileByAdminAsync(int developerId, UpdateDevByAdmin requestBody);
        public Task<GetDevDTO> UpdateDevProfileAsync(int developerId, UpdateDevModel requestBody);
        public Task ChangStatusDeveloperAsync(ChangeStatusDeveloper requestBody);
        public Task<int> GetTotalItemAsync();   
    }
}
