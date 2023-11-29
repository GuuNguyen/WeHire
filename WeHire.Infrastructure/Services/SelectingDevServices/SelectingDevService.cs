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
                var interviewRound = await _unitOfWork.InterviewRepository.Get(i => i.RequestId == requestId &&
                                                                                    i.DeveloperId == dev.DeveloperId &&
                                                                                    i.Status == (int)InterviewStatus.Completed)
                                                                          .CountAsync();
                var mappedDev = _mapper.Map<GetMatchingDev>(dev);
                _mapper.Map(matchingPercentObj, mappedDev);
                var selectedDevStatus = _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId
                                                                             && s.DeveloperId == dev.DeveloperId)
                                                                            .Select(s => s.Status).SingleOrDefault();
                mappedDev.SelectedDevStatus = EnumHelper.GetEnumDescription((SelectedDevStatus)selectedDevStatus);
                mappedDev.InterviewRound = interviewRound++;
                matchingDevs.Add(mappedDev);
            }
            return matchingDevs;
        }

        public async Task<List<GetSelectingDevDTO>> SendDevToHR(SendDevDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository
                                .GetFirstOrDefaultAsync(r => r.RequestId == requestBody.RequestId &&
                                                             r.Status == (int)HiringRequestStatus.InProgress)
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var validDev = _unitOfWork.DeveloperRepository.Get(d => requestBody.DeveloperIds.Contains(d.DeveloperId))
                                                          .Where(d => d.Status == (int)DeveloperStatus.Available &&
                                                                      d.User.RoleId == (int)RoleEnum.Developer);

            if (validDev.Count() != requestBody.DeveloperIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            var selectedDevs = new List<SelectedDev>();
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                selectedDevs = validDev.Select(dev => new SelectedDev
                {
                    Request = request,
                    Developer = dev,
                    Status = (int)SelectedDevStatus.UnderConsideration,
                }).ToList();
                await _unitOfWork.SelectedDevRepository.InsertRangeAsync(selectedDevs);

                foreach (var selectedDev in selectedDevs)
                {
                    var dev = _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == selectedDev.DeveloperId).Include(d => d.User).SingleOrDefault();
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

            var mappedSelectingDev = _mapper.Map<List<GetSelectingDevDTO>>(selectedDevs);
            return mappedSelectingDev;
        }


        public async Task<GetSelectingDevDTO> ChangeStatusApprovalByHRAsync(ChangeSelectingDevStatusDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository.GetFirstOrDefaultAsync(r => r.RequestId == requestBody.RequestId
                                                                                       && r.Status == (int)HiringRequestStatus.InProgress)
                  ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var selectingDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId
                                                                             && s.DeveloperId == requestBody.DeveloperId
                                                                             && s.Status == (int)SelectedDevStatus.UnderConsideration)
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
                        $"You have been approved on request #{request.RequestCode}. Check out the request details!");
                }
                else
                {
                    selectingDev.Status = (int)SelectedDevStatus.Rejected;
                    dev.Status = (int)DeveloperStatus.Available;
                    await _notificationService.SendNotificationAsync(dev.User.UserId, selectingDev.RequestId, NotificationTypeString.HIRING_REQUEST,
                        $"You have been rejected on request #{request.RequestCode}. Check out the request details!");
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


        public async Task<GetSelectingDevDTO> RejectDeveloperAsync(int requestId, int developerId)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestId)
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                              s.DeveloperId == developerId)
                                                                     .Include(s => s.Developer)
                                                                     .SingleOrDefaultAsync();
            if (selectedDev != null)
            {
                selectedDev.Status = (int)SelectedDevStatus.Rejected;
                selectedDev.Developer.Status = (int)DeveloperStatus.Available;
                _unitOfWork.SelectedDevRepository.Update(selectedDev);
                await _unitOfWork.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(selectedDev.Developer.UserId, requestId, NotificationTypeString.HIRING_REQUEST,
                      $"You have been rejected on request #{request.RequestCode}. Check out the request details!");
            }
            var mappedSelectedDev = _mapper.Map<GetSelectingDevDTO>(selectedDev);
            return mappedSelectedDev;
        }
    }
}
