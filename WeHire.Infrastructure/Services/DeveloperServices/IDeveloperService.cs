using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;
using static WeHire.Domain.Enums.DeveloperEnum;

namespace WeHire.Infrastructure.Services.DeveloperServices
{
    public interface IDeveloperService
    {
        public List<GetAllFieldDev> GetAllDev(PagingQuery query, SearchDeveloperDTO searchKey);
        public Task<GetDevDetail> GetDevByIdAsync(int devId);
        public List<GetDevDTO> GetUnofficialDev(PagingQuery query);
        public List<GetAllFieldDev> GetDevsWaitingInterview(PagingQuery query, int requestId);
        public Task<List<GetAllFieldDev>> GetAllDevByTaskIdAsync(int taskId);
        public Task<List<GetMatchingDev>> GetDevMatchingWithRequest(int requestId);
        public Task<GetDevDTO> CreateDevAsync(CreateDevDTO requestBody);
        public Task<GetDevDTO> UpdateDevProfileAsync(int id, UpdateDevProfile requestBody);
        public Task<GetDevDTO> ActiveDeveloperAsync(int developerId);
        public Task<int> GetTotalItemAsync();
        public Task<int> GetTotalDevWaitingInterviewAsync(int requestId);
        public Task<int> GetTotalUnofficialAsync();
       
    }
}
