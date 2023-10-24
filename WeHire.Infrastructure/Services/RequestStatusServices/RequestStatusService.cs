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
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.DTOs.HiringRequest.ChangeStatusDTO;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiringRequestEnum;

namespace WeHire.Infrastructure.Services.RequestStatusServices
{
    public class RequestStatusService : IRequestStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RequestStatusService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetRequestDTO> HandleWaitingStatusAsync(WaitingStatus requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId 
                                                               && r.Status == (int)HiringRequestStatus.WaitingApproval)
                                                             .SingleOrDefaultAsync();
            if (request == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            if (requestBody.isApproved)
            {
                request.Status = (int)HiringRequestStatus.InProgress;
                request.RejectionReason = null;
            }      
            else
            {
                request.Status = (int)HiringRequestStatus.Rejected;
                request.RejectionReason = requestBody.RejectionReason;
            }
            _unitOfWork.RequestRepository.Update(request);
            await _unitOfWork.SaveChangesAsync();

            var mappedRequest = _mapper.Map<GetRequestDTO>(request);
            return mappedRequest;
        }

        public async Task<GetRequestDTO> HandleExpiredStatusAsync(ExpiredStatus requestBody)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId
                                                               && r.Status == (int)HiringRequestStatus.Expired)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            if(requestBody.isExtended)
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
