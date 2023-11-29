using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.NotificationServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiredDeveloperEnum;

namespace WeHire.Infrastructure.Services.HiredDeveloperServices
{
    public class HiredDeveloperService : IHiredDeveloperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public HiredDeveloperService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task CreateHiredDeveloper(int? jobPositionId, int? projectId, Developer developer, int contractId, string projectCode)
        {
            var isExistedDev = await _unitOfWork.HiredDeveloperRepository.AnyAsync(h => h.DeveloperId == developer.DeveloperId && h.ProjectId == projectId);
            if(isExistedDev)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "This developer has been joined the project!");
            }
            var hiredDeveloper = new HiredDeveloper
            {
                ProjectId = projectId,
                DeveloperId = developer.DeveloperId,
                ContractId = contractId,
                JobPositionId = jobPositionId,
                Status = (int)HiredDeveloperStatus.ContractProcessing
            };
            await _unitOfWork.HiredDeveloperRepository.InsertAsync(hiredDeveloper);
            await _notificationService.SendNotificationAsync(developer.UserId, (int)projectId!, NotificationTypeString.CONTRACT,
                                                $"You have been included in project {projectCode}. Please wait for the contract process to complete.");
            await _unitOfWork.SaveChangesAsync();         
        }
    }
}
