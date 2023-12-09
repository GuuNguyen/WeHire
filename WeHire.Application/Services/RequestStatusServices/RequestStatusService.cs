using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.GlobalVariables;
using WeHire.Domain.Enums;
using WeHire.Application.Services.HiringRequestServices;
using WeHire.Application.Services.NotificationServices;
using static WeHire.Application.DTOs.HiringRequest.ChangeStatusDTO;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.InterviewEnum;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.RequestStatusServices
{
    public class RequestStatusService : IRequestStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IHiringRequestService _hiringRequestService;
        public RequestStatusService(IUnitOfWork unitOfWork, IMapper mapper, IHiringRequestService hiringRequestService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
            _hiringRequestService = hiringRequestService;
        }

        public async Task<GetRequestDTO> HandleWaitingStatusAsync(WaitingStatus requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId &&
                                                                       r.Status == (int)HiringRequestStatus.WaitingApproval)
                                                             .Include(r => r.Project)
                                                             .ThenInclude(p => p.Company)
                                                             .SingleOrDefaultAsync();
            if (request == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            if (requestBody.isApproved)
            {
                request.Status = (int)HiringRequestStatus.InProgress;
                await _notificationService.SendNotificationAsync(request.Project.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                    $"Your hiring request {request.RequestCode} has been approved by WeHire.");
            }
            else
            {
                request.Status = (int)HiringRequestStatus.Rejected;
                request.RejectionReason = requestBody.RejectionReason;
                await _notificationService.SendNotificationAsync(request.Project.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                    $"Your hiring request {request.RequestCode} has been rejected by WeHire.");
            }
            _unitOfWork.RequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }

        public async Task<GetRequestDTO> ClosedRequestAsync(CloseRequestModel requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId &&
                                                                (r.Status == (int)HiringRequestStatus.InProgress ||
                                                                 r.Status == (int)HiringRequestStatus.WaitingApproval ||
                                                                 r.Status == (int)HiringRequestStatus.Expired))
                                                             .Include(r => r.Interviews)
                                                             .Include(r => r.Project)
                                                             .ThenInclude(r => r.Company)
                                                             .Include(r => r.HiredDevelopers)
                                                             .ThenInclude(h => h.Developer)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            
            var companyName = requestBody.IsCompanyPartner ? request.Project.Company.CompanyName : "WeHire";

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await _hiringRequestService.HandleDeveloperAfterCloseHiringRequest(request);
                request.RejectionReason = requestBody.RejectionReason;
                await _notificationService.SendNotificationAsync(request.Project.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                                            $"Your hiring request #{request.RequestId} has been closed by {companyName}.");
                _unitOfWork.RequestRepository.Update(request);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }

        public async Task<GetRequestDTO> HandleExpiredStatusAsync(ExpiredStatus requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId &&
                                                                       r.Status == (int)HiringRequestStatus.Expired)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            if (requestBody.NewDuration <= DateTime.Now)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Duration", "New duration must be greater than current date!!");
            }
            request.Duration = requestBody.NewDuration;
            request.Status = (int)HiringRequestStatus.InProgress;
            await _unitOfWork.SaveChangesAsync();

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }
    }
}
