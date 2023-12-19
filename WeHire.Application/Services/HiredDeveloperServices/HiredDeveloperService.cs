using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiredDeveloper;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Application.Services.NotificationServices;
using WeHire.Application.Services.PercentCalculatServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.ContractEnum;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using WeHire.Infrastructure.IRepositories;
using static WeHire.Domain.Enums.InterviewEnum;

namespace WeHire.Application.Services.HiredDeveloperServices
{
    public class HiredDeveloperService : IHiredDeveloperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IPercentCalculateService _percentCalculateService;
        private readonly IMapper _mapper;


        public HiredDeveloperService(IUnitOfWork unitOfWork, INotificationService notificationService, IMapper mapper, IPercentCalculateService percentCalculateService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
            _percentCalculateService = percentCalculateService;
        }

        public async Task<List<GetMatchingDev>> GetDevsInHiringRequest(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                             .AsNoTracking()
                                                             .Include(r => r.SkillRequires)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var devs = await _unitOfWork.HiredDeveloperRepository.Get(s => s.RequestId == requestId)
                                                                 .Include(s => s.Developer.User)
                                                                 .Include(s => s.Developer.Level)
                                                                 .Include(s => s.Developer.DeveloperSkills)
                                                                    .ThenInclude(ds => ds.Skill)
                                                                 .Include(s => s.Developer.DeveloperTypes)
                                                                    .ThenInclude(dt => dt.Type)
                                                                 .Select(s => s.Developer)
                                                                 .ToListAsync();            
            var matchingDevs = new List<GetMatchingDev>();
            foreach (var dev in devs)
            {
                var matchingPercentObj = _percentCalculateService.CalculateMatchingPercentage(request, dev);
                var mappedDev = _mapper.Map<GetMatchingDev>(dev);
                _mapper.Map(matchingPercentObj, mappedDev);
                var hiredDevStatus = _unitOfWork.HiredDeveloperRepository.Get(h => h.RequestId == requestId &&
                                                                                   h.DeveloperId == dev.DeveloperId)
                                                                         .Select(h => h.Status)
                                                                         .SingleOrDefault();
                mappedDev.HiredDeveloperStatus = EnumHelper.GetEnumDescription((HiredDeveloperStatus)hiredDevStatus);
                matchingDevs.Add(mappedDev);
            }
            return matchingDevs;
        }

        public async Task<List<GetHiredDeveloperModel>> SendDevToHR(SendDevDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository
                                .Get(r => r.RequestId == requestBody.RequestId &&
                                          r.Status == (int)HiringRequestStatus.InProgress)
                                .Include(r => r.Project)
                                .SingleOrDefaultAsync() 
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var validDev = _unitOfWork.DeveloperRepository.Get(d => requestBody.DeveloperIds.Contains(d.DeveloperId))
                                                          .Where(d => d.Status == (int)DeveloperStatus.Available &&
                                                                      d.User.RoleId == (int)RoleEnum.Developer);
            var isExistedDevInRequest = await _unitOfWork.HiredDeveloperRepository.AnyAsync(h => requestBody.DeveloperIds.Contains((int)h.DeveloperId) &&
                                                                                            h.RequestId == request.RequestId);
            if(isExistedDevInRequest)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "Developer", "There are one or more developer has been rejected on this request. Please choose another one!");

            if (validDev.Count() != requestBody.DeveloperIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            var hiredDevs = new List<HiredDeveloper>();
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                hiredDevs = validDev.Select(dev => new HiredDeveloper
                {
                    Project = request.Project,
                    Request = request,
                    Developer = dev,
                    Status = (int)HiredDeveloperStatus.UnderConsideration,
                }).ToList();
                await _unitOfWork.HiredDeveloperRepository.InsertRangeAsync(hiredDevs);

                foreach (var hiredDev in hiredDevs)
                {
                    var dev = _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == hiredDev.DeveloperId).Include(d => d.User).SingleOrDefault();
                    dev!.Status = (int)DeveloperStatus.SelectedOnRequest;
                    request.TargetedDev++;
                    await _notificationService.SendNotificationAsync(dev.User.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been selected for a hiring request #{request.RequestCode}. Let's check it out!");
                }
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedHiredDeveloper = _mapper.Map<List<GetHiredDeveloperModel>>(hiredDevs);
            return mappedHiredDeveloper;
        }


        public async Task<GetHiredDeveloperModel> RejectDeveloperAsync(int requestId, int developerId)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var hiredDev = await _unitOfWork.HiredDeveloperRepository.Get(s => s.RequestId == requestId &&
                                                                               s.DeveloperId == developerId &&
                                                                               s.Developer.Status == (int)DeveloperStatus.SelectedOnRequest)
                                                                     .Include(s => s.Developer)
                                                                     .Include(s => s.Interviews)
                                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper does not exist!");

            if (hiredDev != null)
            {
                hiredDev.Status = (int)HiredDeveloperStatus.Rejected;
                hiredDev.Developer.Status = (int)DeveloperStatus.Available;

                var interviews = hiredDev.Interviews.Where(i => i.Status == (int)InterviewStatus.WaitingDevApproval ||
                                                                i.Status == (int)InterviewStatus.Approved)
                                                    .ToList();
                foreach(var interview in interviews)
                {
                    interview.Status = (int)InterviewStatus.Cancelled;
                    await _notificationService.SendNotificationAsync(hiredDev.Developer.UserId, interview.InterviewId, NotificationTypeString.INTERVIEW,
                              $"Interview {interview.InterviewId} has been cancelled!");
                }

                await _unitOfWork.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(hiredDev.Developer.UserId, requestId, NotificationTypeString.HIRING_REQUEST,
                      $"You have been rejected on request #{request.RequestCode}. Check out the request details!");
            }
            var mappedHiredDeveloper = _mapper.Map<GetHiredDeveloperModel>(hiredDev);
            return mappedHiredDeveloper;
        }

        public async Task<GetHiredDeveloperModel> TerminateDeveloperAsync(int projectId, int developerId)
        {
            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.DeveloperId == developerId &&
                                                                               h.ProjectId == projectId &&
                                                                               h.Status == (int)HiredDeveloperStatus.Working)
                                                                           .Include(h => h.Developer)
                                                                           .Include(h => h.Contract)
                                                                           .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper does not exist!");

            hiredDeveloper.Status = (int)HiredDeveloperStatus.Terminated;
            hiredDeveloper.Developer.Status = (int)DeveloperStatus.Available;
            hiredDeveloper.Contract.Status = (int)ContractStatus.Terminated;
            await _unitOfWork.SaveChangesAsync();
            var mappedHiredDeveloper = _mapper.Map<GetHiredDeveloperModel>(hiredDeveloper);
            return mappedHiredDeveloper;
        }
    }
}
