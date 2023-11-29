using AutoMapper;
using Google.Apis.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.Interview;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.NotificationServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.InterviewEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;
using static WeHire.Domain.Enums.UserEnum;
using ChangeStatusDTO = WeHire.Application.DTOs.Interview.ChangeStatusDTO;

namespace WeHire.Infrastructure.Services.InterviewServices
{
    internal class InterviewService : IInterviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public InterviewService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public List<GetListInterview> GetInterviewsByManager(PagingQuery query, int? companyId, SearchInterviewWithRequest searchKey)
        {
            var interviews = _unitOfWork.InterviewRepository.Get()
                                                            .Include(i => i.Request)
                                                            .ThenInclude(i => i.Company)
                                                            .AsQueryable();
            if (companyId.HasValue) interviews.Where(i => i.Request.CompanyId == companyId.Value);

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetListInterview>>(interviews);
            return mappedInterviews;
        }

        public List<GetListInterview> GetInterviewsByCompany(int? companyId, int? requestId, PagingQuery query, SearchInterviewDTO searchKey)
        {
            if (!companyId.HasValue) return new();
            var interviews = _unitOfWork.InterviewRepository.Get(i => i.Request.CompanyId == companyId)
                                                           .Include(i => i.Request)
                                                           .AsQueryable();

            if (requestId.HasValue) interviews = interviews.Where(i => i.RequestId == requestId);

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetListInterview>>(interviews);
            return mappedInterviews;
        }

        public async Task<GetInterviewWithDev> GetInterviewById(int interviewId)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == interviewId)
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);

            var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == interview.DeveloperId)
                                                           .Include(d => d.User)
                                                           .Include(d => d.Gender)
                                                           .Include(d => d.Level)
                                                           .Include(d => d.EmploymentType)
                                                           .Include(d => d.DeveloperSkills)
                                                                .ThenInclude(ds => ds.Skill)
                                                           .Include(d => d.DeveloperTypes)
                                                                .ThenInclude(dt => dt.Type)
                                                           .SingleOrDefaultAsync();

            var mappedInterview = _mapper.Map<GetInterviewWithDev>(interview);
            mappedInterview.Developer = _mapper.Map<GetAllFieldDev>(dev);
            return mappedInterview;
        }

        public async Task<List<GetListInterview>> GetInterviewByRequestIdAsync(int requestId)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.RequestId == requestId).AsNoTracking()
                                                                 .ToListAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            var mappedInterview = _mapper.Map<List<GetListInterview>>(interview);
            return mappedInterview;
        }

        public async Task<List<GetListInterview>> GetInterviewByDevId(int devId)
        {
            var requestIds = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == devId)
                                                                    .Select(s => s.RequestId)
                                                                    .ToListAsync();
            var interviews = await _unitOfWork.InterviewRepository.Get(i => requestIds.Contains((int)i.RequestId!)).ToListAsync();
            var mappedInterviews = _mapper.Map<List<GetListInterview>>(interviews);
            return mappedInterviews;
        }

        public async Task<GetInterviewDTO> CreateInterviewAsync(CreateInterviewDTO requestBody)
        {
            var request = await _unitOfWork.RequestRepository.GetByIdAsync(requestBody.RequestId);
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(requestBody.DeveloperId);

            var newInterview = _mapper.Map<Interview>(requestBody);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                newInterview.InterviewCode = await GenerateUniqueCodeName();
                newInterview.Status = (int)InterviewStatus.WaitingDevApproval;
                newInterview.CreatedAt = DateTime.Now;
                newInterview.NumOfInterviewee = 1;
                newInterview.EventId = Guid.NewGuid().ToString();
                await HandleSelectedDevToInterviewing(requestBody.RequestId, requestBody.DeveloperId);
                await _unitOfWork.InterviewRepository.InsertAsync(newInterview);
                await _unitOfWork.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(dev.UserId, newInterview.InterviewId, NotificationTypeString.INTERVIEW,
                $"You have been invited for an interview for request {request.RequestCode}. Check out the interview details!");
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedInterview = _mapper.Map<GetInterviewDTO>(newInterview);
            return mappedInterview;
        }

        public async Task<GetInterviewDTO> UpdateInterviewAsync(int interviewId, UpdateInterviewModel requestBody)
        {
            if(interviewId != requestBody.InterviewId)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "InterviewId", "interviewId not match with each other!!");
            }
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == interviewId && 
                                                                           i.Status == (int)InterviewStatus.WaitingDevApproval)
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);

            var updatedInterview = _mapper.Map(requestBody, interview);
            _unitOfWork.InterviewRepository.Update(updatedInterview);
            await _unitOfWork.SaveChangesAsync();

            var mappedInterview = _mapper.Map<GetInterviewDTO>(updatedInterview);
            return mappedInterview;
        }

        public async Task<GetInterviewDTO> CancelInterviewAsync(int interviewId)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == interviewId &&
                                                                          (i.Status == (int)InterviewStatus.WaitingDevApproval ||
                                                                           i.Status == (int)InterviewStatus.Approved))
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == interview.DeveloperId &&
                                                                               s.RequestId == interview.RequestId)
                                                                     .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);

            interview.Status = (int)InterviewStatus.Cancelled;
            interview.MeetingUrl = null;
            interview.OutlookUrl = null;
            selectedDev.Status = (int)SelectedDevStatus.UnderConsideration;

            await _unitOfWork.SaveChangesAsync();

            var mappedInterview = _mapper.Map<GetInterviewDTO>(interview);
            return mappedInterview;
        }

        private async Task HandleSelectedDevToInterviewing(int requestId, int developerId)
        {
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == developerId && s.RequestId == requestId)
                                                                     .SingleOrDefaultAsync()
                  ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            selectedDev.Status = (int)SelectedDevStatus.InterviewScheduled;
            _unitOfWork.SelectedDevRepository.Update(selectedDev);
        }
       

        public async Task<GetInterviewDTO> ChangeStatusAsync(ChangeStatusDTO requestBody)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == requestBody.InterviewId &&
                                                                           i.Status == (int)InterviewStatus.WaitingDevApproval)
                                                                 .Include(i => i.Request)
                                                                 .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == interview.DeveloperId
                                                                            && s.RequestId == interview.RequestId)
                                                                     .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (requestBody.isApproved)
                {
                    interview.Status = (int)InterviewStatus.Approved;
                }
                else
                {
                    interview.Status = (int)InterviewStatus.Rejected;
                    interview.RejectionReason = requestBody.RejectionReason;
                    interview.MeetingUrl = null;
                    interview.OutlookUrl = null;
                    selectedDev.Status = (int)SelectedDevStatus.UnderConsideration;
                }

                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            return _mapper.Map<GetInterviewDTO>(interview);
        }
       

        public async Task<GetInterviewDTO> FinishInterviewAsync(int interviewId)
        {
            var interview = await _unitOfWork.InterviewRepository.GetByIdAsync(interviewId)
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == interview.DeveloperId
                                                                            && s.RequestId == interview.RequestId)
                                                                     .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);
            DateTime interviewDate = interview.DateOfInterview!.Value.Date;

            TimeSpan endTime = interview.EndTime!.Value;

            DateTime combinedDateTime = interviewDate.Add(endTime);

            DateTime currentDateTime = DateTime.Now;

            if (combinedDateTime < currentDateTime)
            {
                interview.Status = (int)InterviewStatus.Completed;
                selectedDev.Status = (int)SelectedDevStatus.WaitingInterview;
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "EndTime", "The interview must be out of time before it can finished");
            }
            var mappedInterview = _mapper.Map<GetInterviewDTO>(interview);
            return mappedInterview;
        }
        public async Task<int> GetTotalInterviewsAsync(int? companyId = null)
        {
            IQueryable<Interview> interviewQuery = _unitOfWork.InterviewRepository.Get().Include(i => i.Request);
            if (companyId.HasValue)
            {
                interviewQuery = interviewQuery.Where(i => i.Request.CompanyId == companyId.Value);
            }

            var total = await interviewQuery.CountAsync();

            return total;
        }

        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistInterviewCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "INV" + randomNumber.ToString();
                isExistInterviewCode = await _unitOfWork.InterviewRepository.AnyAsync(d => d.InterviewCode == codeName);
            } while (isExistInterviewCode);

            return codeName;
        }

        public async Task ExpireInterviewAsync(DateTime currentTime)
        {
            var interviews = await _unitOfWork.InterviewRepository.Get()
                    .Where(i => i.DateOfInterview.HasValue && i.StartTime.HasValue && i.Status == (int)InterviewStatus.WaitingDevApproval)
                    .ToListAsync();

            var filteredInterviews = interviews
                .Where(i => i.DateOfInterview.Value.Date + i.StartTime.Value < currentTime)
                .ToList();

            if (filteredInterviews.Any())
            {
                foreach (var interview in filteredInterviews)
                {
                    var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == interview.DeveloperId &&
                                                                                      s.RequestId == interview.RequestId)
                                                                                     .SingleOrDefaultAsync();
                    interview.Status = (int)InterviewStatus.Rejected;
                    interview.RejectionReason = $"WeHire: This developer has not responded to the interview request, please create a new interview.";
                    interview.MeetingUrl = null;
                    interview.OutlookUrl = null;
                    selectedDev.Status = (int)SelectedDevStatus.UnderConsideration;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
    }
}
