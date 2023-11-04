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
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.NotificationServices;
using static WeHire.Application.DTOs.HiringRequest.ChangeStatusDTO;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;

namespace WeHire.Infrastructure.Services.RequestStatusServices
{
    public class RequestStatusService : IRequestStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public RequestStatusService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<GetRequestDTO> HandleWaitingStatusAsync(WaitingStatus requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId
                                                               && r.Status == (int)HiringRequestStatus.WaitingApproval)
                                                             .Include(r => r.Company)
                                                             .SingleOrDefaultAsync();
            if (request == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            if (requestBody.isApproved)
            {
                request.Status = (int)HiringRequestStatus.InProgress;
                await _notificationService.SendNotificationAsync(request.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                    $"Your hiring request #{request.RequestId} has been approved by WeHire.");
            }
            else
            {
                request.Status = (int)HiringRequestStatus.Rejected;
                request.RejectionReason = requestBody.RejectionReason;
                await _notificationService.SendNotificationAsync(request.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                    $"Your hiring request #{request.RequestId} has been rejected by WeHire.");
            }
            _unitOfWork.RequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }

        public async Task<GetRequestDTO> CancelRequestAsync(CancelRequestModel requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId)
                                                             .Include(r => r.Company)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            var devIds = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId).Select(s => s.DeveloperId).ToListAsync();
            var companyName = requestBody.IsCompanyPartner ? request.Company.CompanyName : "WeHire";
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                request.Status = (int)HiringRequestStatus.Rejected;
                request.RejectionReason = requestBody.RejectionReason;
                if (devIds.Any())
                {
                    var devs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == request.RequestId &&
                                                                                devIds.Contains(s.DeveloperId))
                                                            .Include(s => s.Developer)
                                                            .Select(s => s.Developer)
                                                            .ToListAsync();
                    foreach (var dev in devs)
                    {
                        dev.Status = (int)DeveloperStatus.Available;

                        await _notificationService.SendNotificationAsync(dev.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                              $"The hiring request #{request.RequestId} has been cancelled by {companyName}.");
                    }
                }
                await _notificationService.SendNotificationAsync(request.Company.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                                            $"Your hiring request #{request.RequestId} has been cancelled by {companyName}.");
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
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId
                                                               && r.Status == (int)HiringRequestStatus.Expired)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            if (requestBody.isExtended)
            {
                request.Duration = requestBody.NewDuration;
                request.Status = (int)HiringRequestStatus.InProgress;
            }
            else
            {
                request.Status = (int)HiringRequestStatus.Finished;
            }
            _unitOfWork.RequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }
    }
}
