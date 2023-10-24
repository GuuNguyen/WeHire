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

namespace WeHire.Infrastructure.Services.SelectingDevServices
{
    public class SelectingDevService : ISelectingDevService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPercentCalculateService _percentCalculateService;
        private readonly IMapper _mapper;

        public SelectingDevService(IUnitOfWork unitOfWork, IMapper mapper, IPercentCalculateService percentCalculateService)
        {
            _unitOfWork = unitOfWork;
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

            var devs = _unitOfWork.DeveloperRepository.Get(d => selectedDev.DeveloperIds.Contains(d.DeveloperId));

            var selectedDevs = await devs.Select(dev => new SelectedDev
                                        {
                                            Request = request,
                                            Developer = dev,
                                            Status = (int)SelectedDevStatus.WaitingDevAccept,
                                        }).ToListAsync();

            await _unitOfWork.SelectedDevRepository.InsertRangeAsync(selectedDevs);
            await _unitOfWork.SaveChangesAsync();

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

            if(selectedDevs.Count != requestBody.DeveloperIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                foreach (var selectedDev in selectedDevs)
                {
                    selectedDev.Status = (int)SelectedDevStatus.WaitingHRAccept;
                    var dev = _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == selectedDev.DeveloperId).SingleOrDefault();
                    dev!.Status = (int)DeveloperStatus.SelectedOnRequest;
                    request.TargetedDev++;
                    _unitOfWork.RequestRepository.Update(request);
                    _unitOfWork.DeveloperRepository.Update(dev);
                    _unitOfWork.SelectedDevRepository.Update(selectedDev);
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
                                                                      && s.Status == (int)SelectedDevStatus.WaitingHRAccept
                                                                     ).SingleOrDefaultAsync();
            if (selectingDev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);          
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(requestBody.DeveloperId);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (requestBody.isApproved)
                {
                    selectingDev.Status = (int)SelectedDevStatus.WaitingInterview;
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
            catch(Exception)
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
                selectedDev.Status = (int)SelectedDevStatus.WaitingHRAccept;

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
            }         
            var mappedSelectedDev = _mapper.Map<GetSelectingDevDTO>(selectedDev);
            return mappedSelectedDev;
        }

        public async Task<List<GetSelectingDevDTO>> ChangeStatusToInterviewingAsync(ChangeStatusToInterviewingDTO requestBody)
        {
            var selectedDevs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId &&
                                                                                s.Status == (int)SelectedDevStatus.WaitingInterview &&
                                                                                s.Request.Status == (int)HiringRequestStatus.InProgress)
                                                                      .Where(s => requestBody.DevIds.Contains(s.DeveloperId) &&
                                                                                s.Developer.Status == (int)DeveloperStatus.SelectedOnRequest)
                                                                      .Include(s => s.Developer)
                                                                      .Include(s => s.Request)
                                                                      .ToListAsync();
            if(selectedDevs.Count() != requestBody.DevIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            var interview = _unitOfWork.InterviewRepository.Get(i => i.InterviewId == requestBody.InterviewId && i.Status == (int)InterviewStatus.WaitingManagerApproval)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);

            selectedDevs.ForEach(d =>
            {
                d.Status = (int)SelectedDevStatus.Interviewing;
                d.InterviewId = requestBody.InterviewId;
                _unitOfWork.SelectedDevRepository.Update(d);
            });     
            await _unitOfWork.SaveChangesAsync();

            var mappedDevs = _mapper.Map<List<GetSelectingDevDTO>>(selectedDevs); 
            return mappedDevs;
        }
    }
}
