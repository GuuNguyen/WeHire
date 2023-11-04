using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeHire.Application.DTOs.SelectingDev;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.DTOs.Developer;
using WeHire.Infrastructure.Services.DeveloperServices;
using WeHire.Infrastructure.Services.PercentCalculatServices;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.InterviewEnum;
using WeHire.Infrastructure.Services.NotificationServices;
using WeHire.Application.Utilities.GlobalVariables;

namespace WeHire.Infrastructure.Services.SelectingDevServices
{
    public class SelectingDevService : ISelectingDevService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPercentCalculateService _percentCalculateService;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public SelectingDevService(IUnitOfWork unitOfWork, IMapper mapper, IPercentCalculateService percentCalculateService,
                                   INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
            _percentCalculateService = percentCalculateService;
        }

        public async Task<List<Developer>> GetSelectedDevsById(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var devs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId)
                                                                .Include(s => s.Developer.User)
                                                                .Include(s => s.Developer.Level)
                                                                .Include(s => s.Developer.DeveloperSkills)
                                                                    .ThenInclude(ds => ds.Skill)
                                                                .Include(s => s.Developer.DeveloperTypes)
                                                                    .ThenInclude(dt => dt.Type)
                                                                .Select(s => s.Developer)
                                                                .ToListAsync();
            return devs;
        }

        public async Task<List<GetMatchingDev>> GetSelectedDevsForManager(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                              .AsNoTracking()
                                                              .Include(r => r.SkillRequires)
                                                              .SingleOrDefaultAsync()
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var devs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId)
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
                var selectedDevStatus = _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId
                                                                             && s.DeveloperId == dev.DeveloperId)
                                                                            .Select(s => s.Status).SingleOrDefault();
                mappedDev.SelectedDevStatus = EnumHelper.GetEnumDescription((SelectedDevStatus)selectedDevStatus);
                matchingDevs.Add(mappedDev);
            }

            return matchingDevs;
        }

        public async Task<List<GetMatchingDev>> GetSelectedDevsForHR(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                              .AsNoTracking()
                                                              .Include(r => r.SkillRequires)
                                                              .SingleOrDefaultAsync()
                  ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var devs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId)
                                                              .Where(s => s.Status != (int)SelectedDevStatus.WaitingDevAccept &&
                                                                          s.Status != (int)SelectedDevStatus.DevAccepted &&
                                                                          s.Status != (int)SelectedDevStatus.DevRejected)
                                                              .Include(s => s.Developer.User)
                                                              .Include(s => s.Developer.Level)
                                                              .Include(s => s.Developer.DeveloperTypes)
                                                                    .ThenInclude(dt => dt.Type)
                                                                  .Include(s => s.Developer.DeveloperSkills)
                                                                    .ThenInclude(ds => ds.Skill)
                                                              .Select(s => s.Developer)
                                                              .ToListAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.LIST_DEV_NULL);

            var matchingDevs = new List<GetMatchingDev>();
            foreach (var dev in devs)
            {
                var matchingPercentObj = _percentCalculateService.CalculateMatchingPercentage(request, dev);
                var mappedDev = _mapper.Map<GetMatchingDev>(dev);
                _mapper.Map(matchingPercentObj, mappedDev);
                var selectedDevStatus = _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId
                                                                             && s.DeveloperId == dev.DeveloperId)
                                                                            .Select(s => s.Status).SingleOrDefault();
                mappedDev.SelectedDevStatus = EnumHelper.GetEnumDescription((SelectedDevStatus)selectedDevStatus);
                matchingDevs.Add(mappedDev);
            }
            return matchingDevs;
        }

        public async Task<string> CreateSelectDevForRequest(CreateSelectedDev selectedDev)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == selectedDev.RequestId
                                                                    && r.Status == (int)HiringRequestStatus.InProgress)
                                                             .SingleOrDefaultAsync();

            if (request == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var validDevIds = _unitOfWork.DeveloperRepository.Get(d => selectedDev.DeveloperIds.Contains(d.DeveloperId))
                                                             .Where(d => d.Status == (int)DeveloperStatus.Available
                                                                    && d.User.RoleId == (int)RoleEnum.Developer);

            if (validDevIds.Count() != selectedDev.DeveloperIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            var devs = _unitOfWork.DeveloperRepository.Get(d => selectedDev.DeveloperIds.Contains(d.DeveloperId)).ToList();
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                var selectedDevs = devs.Select(dev => new SelectedDev
                {
                    Request = request,
                    Developer = dev,
                    Status = (int)SelectedDevStatus.WaitingDevAccept,
                }).ToList();

                foreach (var d in devs)
                {
                    await _notificationService.SendNotificationAsync(d.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been invited for a hiring request #{request.RequestId}. The request requires your decision!");
                }
                await _unitOfWork.SelectedDevRepository.InsertRangeAsync(selectedDevs);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
         
            return ResponseMessage.CREATE_SELECTED_DEV_SUCCESS;
        }


        public async Task<List<GetSelectingDevDTO>> SendDevToHR(SendDevDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository.GetFirstOrDefaultAsync(r =>
                                                            r.RequestId == requestBody.RequestId &&
                                                            r.Status == (int)HiringRequestStatus.InProgress)
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var developerIds = requestBody.DeveloperIds.ToHashSet();
            var selectedDevs = await _unitOfWork.SelectedDevRepository
                                                .Get(s => s.RequestId == requestBody.RequestId
                                                    && developerIds.Contains(s.DeveloperId))
                                                .Where(s => s.Status == (int)SelectedDevStatus.DevAccepted)
                                                .ToListAsync();

            if (selectedDevs.Count != requestBody.DeveloperIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                foreach (var selectedDev in selectedDevs)
                {
                    selectedDev.Status = (int)SelectedDevStatus.WaitingHRAccept;
                    var dev = _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == selectedDev.DeveloperId).Include(d => d.User).SingleOrDefault();
                    dev!.Status = (int)DeveloperStatus.SelectedOnRequest;
                    request.TargetedDev++;
                    _unitOfWork.RequestRepository.Update(request);
                    _unitOfWork.DeveloperRepository.Update(dev);
                    _unitOfWork.SelectedDevRepository.Update(selectedDev);
                    await _notificationService.SendNotificationAsync(dev.User.UserId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been selected for a hiring request #{request.RequestId}. Let's check it out!");
                }
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedSelectingDev = _mapper.Map<List<GetSelectingDevDTO>>(selectedDevs);
            return mappedSelectingDev;
        }

        public async Task<GetSelectingDevDTO> ChangeStatusApprovalByDevAsync(ChangeSelectingDevStatusDTO requestBody)
        {
            var selectingDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId
                                                                     && s.DeveloperId == requestBody.DeveloperId
                                                                     && s.Status == (int)SelectedDevStatus.WaitingDevAccept
                                                                    ).SingleOrDefaultAsync();
            if (selectingDev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);

            selectingDev.Status = requestBody.isApproved ? (int)SelectedDevStatus.DevAccepted : (int)SelectedDevStatus.DevRejected;
            _unitOfWork.SelectedDevRepository.Update(selectingDev);
            await _unitOfWork.SaveChangesAsync();

            var mappedSelectingDev = _mapper.Map<GetSelectingDevDTO>(selectingDev);
            return mappedSelectingDev;
        }

        public async Task<GetSelectingDevDTO> ChangeStatusApprovalByHRAsync(ChangeSelectingDevStatusDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository.GetFirstOrDefaultAsync(r => r.RequestId == requestBody.RequestId
                                                                                       && r.Status == (int)HiringRequestStatus.InProgress)
                  ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var selectingDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId
                                                                             && s.DeveloperId == requestBody.DeveloperId
                                                                             && s.Status == (int)SelectedDevStatus.WaitingHRAccept)
                                                                      .SingleOrDefaultAsync();
            if (selectingDev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);


            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == requestBody.DeveloperId).Include(d => d.User).SingleOrDefaultAsync();

                if (requestBody.isApproved)
                {
                    selectingDev.Status = (int)SelectedDevStatus.WaitingInterview;
                    await _notificationService.SendNotificationAsync(dev.User.UserId, selectingDev.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been approved on request #{selectingDev.RequestId}. Check out the request details!");
                }
                else
                {
                    selectingDev.Status = (int)SelectedDevStatus.HRRejected;
                    dev.Status = (int)DeveloperStatus.Available;
                    await _notificationService.SendNotificationAsync(dev.User.UserId, selectingDev.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been rejected on request #{selectingDev.RequestId}. Check out the request details!");
                }
                _unitOfWork.DeveloperRepository.Update(dev);
                _unitOfWork.SelectedDevRepository.Update(selectingDev);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedSelectingDev = _mapper.Map<GetSelectingDevDTO>(selectingDev);
            return mappedSelectingDev;
        }

        public async Task<GetSelectingDevDTO> ChangeStatusToOnboardingAsync(ChangeSelectingDevStatusDTO requestBody)
        {
            var selectingDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId
                                                                     && s.DeveloperId == requestBody.DeveloperId
                                                                     && s.Status == (int)SelectedDevStatus.Interviewing
                                                                    ).SingleOrDefaultAsync();
            if (selectingDev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(requestBody.DeveloperId);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (requestBody.isApproved)
                {
                    selectingDev.Status = (int)SelectedDevStatus.OnBoarding;
                    dev.Status = (int)DeveloperStatus.OnWorking;
                }
                else
                {
                    selectingDev.Status = (int)SelectedDevStatus.HRRejected;
                    dev.Status = (int)DeveloperStatus.Available;
                }
                _unitOfWork.DeveloperRepository.Update(dev);
                _unitOfWork.SelectedDevRepository.Update(selectingDev);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedSelectingDev = _mapper.Map<GetSelectingDevDTO>(selectingDev);
            return mappedSelectingDev;
        }

        public async Task<GetSelectingDevDTO> RemoveOutOfListWaitingInterviewAsync(int requestId, int developerId)
        {
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                               s.DeveloperId == developerId &&
                                                                               s.Status == (int)SelectedDevStatus.WaitingInterview)
                                                                     .SingleOrDefaultAsync();
            if (selectedDev != null)
            {
                selectedDev.Status = (int)SelectedDevStatus.WaitingHRAccept;
                await _unitOfWork.SaveChangesAsync();
            }

            var mappedSelectedDev = _mapper.Map<GetSelectingDevDTO>(selectedDev);
            return mappedSelectedDev;
        }

        public async Task<GetSelectingDevDTO> RejectDeveloperAsync(int requestId, int developerId)
        {
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                              s.DeveloperId == developerId)
                                                                     .Include(s => s.Developer)
                                                                     .SingleOrDefaultAsync();
            if (selectedDev != null)
            {
                selectedDev.Status = (int)SelectedDevStatus.HRRejected;
                selectedDev.Developer.Status = (int)DeveloperStatus.Available;
                _unitOfWork.SelectedDevRepository.Update(selectedDev);
                await _unitOfWork.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(selectedDev.Developer.UserId, requestId, NotificationTypeString.HIRING_REQUEST,
                      $"You have been rejected on request #{requestId}. Check out the request details!");
            }
            var mappedSelectedDev = _mapper.Map<GetSelectingDevDTO>(selectedDev);
            return mappedSelectedDev;
        }

        public async Task ChangeStatusToInterviewingAsync(int requestId)
        {
            var selectedDevs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                                s.Status == (int)SelectedDevStatus.WaitingInterview &&
                                                                                s.Request.Status == (int)HiringRequestStatus.InProgress)
                                                                      .Include(s => s.Developer)
                                                                      .Include(s => s.Request)
                                                                      .ToListAsync();

            selectedDevs.ForEach(d =>
            {
                d.Status = (int)SelectedDevStatus.Interviewing;
                _unitOfWork.SelectedDevRepository.Update(d);
            });
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
