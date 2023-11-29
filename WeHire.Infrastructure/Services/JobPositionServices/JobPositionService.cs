using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.JobPosition;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.JobPositionEnum;

namespace WeHire.Infrastructure.Services.JobPositionServices
{
    public class JobPositionService : IJobPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobPositionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetJobPosition>> GetJobPositionByProjectId(int projectId)
        {
            var jobPositions = await _unitOfWork.JobPositionRepository.Get(j => j.ProjectId == projectId &&
                                                                                j.Status == (int)JobPositionStatus.Active)
                                                                      .ToListAsync();
            var jobPositionMapped = _mapper.Map<List<GetJobPosition>>(jobPositions);
            return jobPositionMapped;
        }

        public async Task<List<GetJpRequestModel>> GetJpWithHiringRequest(int projectId)
        {
            var jobPositions = await _unitOfWork.JobPositionRepository.Get(j => j.ProjectId == projectId &&
                                                                               j.Status == (int)JobPositionStatus.Active)
                                                                      .Include(j => j.HiringRequests)
                                                                      .ToListAsync();
            var jpRequests = new List<GetJpRequestModel>();
            jobPositions.ForEach(j => 
            {
                var jpRequest = new GetJpRequestModel
                {
                    JobPositionId = j.JobPositionId,
                    PositionName = j.PositionName,
                    TotalHiringRequest = j.HiringRequests.Count,
                    RequestsInJobPosition = _mapper.Map<List<GetRequestInJobPosition>>(j.HiringRequests)
                };
                jpRequests.Add(jpRequest);
            });
            return jpRequests;
        }

        public async Task<GetJobPosition> CreateJobPosition(CreateJobPosition requestBody)
        {
            var newJobPosition = _mapper.Map<JobPosition>(requestBody);
            newJobPosition.Status = (int)JobPositionStatus.Active;
            newJobPosition.CreatedAt = DateTime.Now;
            await _unitOfWork.JobPositionRepository.InsertAsync(newJobPosition);
            await _unitOfWork.SaveChangesAsync();

            var jobPositionMapped = _mapper.Map<GetJobPosition>(newJobPosition);
            return jobPositionMapped;
        }

        public async Task<GetJobPosition> UpdateJobPosition(int jobPositionId, UpdateJobPosition requestBody)
        {
            if(jobPositionId != requestBody.JobPositionId)
               throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.JOB_POSITION_FIELD, "JobPositionId not match!");

            var jobPosition = await _unitOfWork.JobPositionRepository.GetByIdAsync(requestBody.JobPositionId);
            var updateJobPosition = _mapper.Map(requestBody, jobPosition);
            updateJobPosition.UpdatedAt = DateTime.Now;
            _unitOfWork.JobPositionRepository.Update(updateJobPosition);
            await _unitOfWork.SaveChangesAsync();

            var jobPositionMapped = _mapper.Map<GetJobPosition>(updateJobPosition);
            return jobPositionMapped;
        }

        public async Task<GetJobPosition> DeleteJobPosition(int jobPositionId)
        {
            var jobPosition = await _unitOfWork.JobPositionRepository.Get(j => j.JobPositionId == jobPositionId && j.Status == (int)JobPositionStatus.Active)
                                                                     .Include(j => j.HiringRequests)
                                                                     .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.JOB_POSITION_FIELD, ErrorMessage.JOB_POSITION_NOT_EXIST);

            if (jobPosition.HiringRequests.Any(h => h.Status == (int)HiringRequestStatus.InProgress || 
                                                    h.Status == (int)HiringRequestStatus.Expired))
            {
                 throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiringRequest", "You must complete or close all requests before you want to delete this job position!");
            }
            if(jobPosition.HiringRequests.Any(h => h.Status == (int)HiringRequestStatus.WaitingApproval))
            {
                foreach(var request in jobPosition.HiringRequests)
                {
                    request.Status = (int)HiringRequestStatus.Rejected;
                    request.RejectionReason = $"Job position {jobPosition.PositionName} has been deleted, resulting in this job request being rejected.";
                }
            }
            jobPosition.Status = (int)JobPositionStatus.Inactive;
            await _unitOfWork.SaveChangesAsync();

            var jobPositionMapped = _mapper.Map<GetJobPosition>(jobPosition);
            return jobPositionMapped;
        }     
    }
}
