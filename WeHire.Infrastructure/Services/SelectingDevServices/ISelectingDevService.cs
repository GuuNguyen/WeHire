using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.SelectingDev;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.SelectingDevServices
{
    public interface ISelectingDevService
    {
        public Task<List<GetMatchingDev>> GetSelectedDevsForManager(int requestId);
        public Task<List<Developer>> GetSelectedDevsById(int requestId);
        public Task<List<GetMatchingDev>> GetSelectedDevsForHR(int requestId);
        public Task<string> CreateSelectDevForRequest(CreateSelectedDev selectedDev);
        public Task<List<GetSelectingDevDTO>> SendDevToHR(SendDevDTO requestBody);
        public Task<GetSelectingDevDTO> ChangeStatusApprovalByDevAsync(ChangeSelectingDevStatusDTO requestBody);
        public Task<GetSelectingDevDTO> ChangeStatusApprovalByHRAsync(ChangeSelectingDevStatusDTO requestBody);
        public Task<GetSelectingDevDTO> ChangeStatusToOnboardingAsync(ChangeSelectingDevStatusDTO requestBody);
        public Task<List<GetSelectingDevDTO>> ChangeStatusToInterviewingAsync(ChangeStatusToInterviewingDTO requestBody);
        public Task<GetSelectingDevDTO> RemoveOutOfListWaitingInterviewAsync(int requestId, int developerId);
        public Task<GetSelectingDevDTO> RejectDeveloperAsync(int requestId, int developerId);
    }
}
