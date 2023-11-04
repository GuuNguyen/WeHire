using AutoMapper;
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

        public List<GetInterviewDetail> GetInterviewsByManager(PagingQuery query, int? companyId, SearchInterviewWithRequest searchKey)
        {
            var interviews = _unitOfWork.InterviewRepository.Get().Include(i => i.Request)
                                                            .AsQueryable();
            if (companyId.HasValue) interviews.Where(i => i.Request.CompanyId == companyId.Value);

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetInterviewDetail>>(interviews);
            return mappedInterviews;
        }

        public List<GetInterviewDetail> GetInterviewsByCompany(int? companyId, int? requestId, PagingQuery query, SearchInterviewDTO searchKey)
        {
            if (!companyId.HasValue) return new();
            var interviews = _unitOfWork.InterviewRepository.Get(i => i.Request.CompanyId == companyId)
                                                            .Include(i => i.Request)
                                                            .AsQueryable();

            if (requestId.HasValue) interviews = interviews.Where(i => i.RequestId == requestId);

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetInterviewDetail>>(interviews);
            return mappedInterviews;
        }

        public async Task<GetInterviewWithDev> GetInterviewById(int interviewId, PagingQuery query)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == interviewId)
                                                                 .Include(i => i.AssignStaff)
                                                                 .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);

            var devs = _unitOfWork.DeveloperInterviewRepository.Get(s => s.InterviewId == interviewId)
                                                                .Include(r => r.Developer.EmploymentType)
                                                                .Include(r => r.Developer.ScheduleType)
                                                                .Include(s => s.Developer.Level)
                                                                .Include(s => s.Developer.DeveloperTypes)
                                                                    .ThenInclude(dt => dt.Type)
                                                                .Include(s => s.Developer.DeveloperSkills)
                                                                    .ThenInclude(ds => ds.Skill)
                                                                .Select(s => s.Developer);

            devs = devs.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterview = _mapper.Map<GetInterviewWithDev>(interview);
            mappedInterview.Developers = _mapper.Map<List<GetAllFieldDev>>(devs.ToList());
            mappedInterview.AssignStaff = _mapper.Map<GetUserDTO>(interview.AssignStaff);
            return mappedInterview;
        }

        public async Task<List<GetInterviewDetail>> GetInterviewByRequestIdAsync(int requestId)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.RequestId == requestId).AsNoTracking()
                                                                 .ToListAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            var mappedInterview = _mapper.Map<List<GetInterviewDetail>>(interview);
            return mappedInterview;
        }

        public async Task<List<GetInterviewDetail>> GetInterviewByDevId(int devId)
        {
            var requestIds = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == devId)
                                                                    .Select(s => s.RequestId)
                                                                    .ToListAsync();
            var interviews = await _unitOfWork.InterviewRepository.Get(i => requestIds.Contains((int)i.RequestId!)).ToListAsync();
            var mappedInterviews = _mapper.Map<List<GetInterviewDetail>>(interviews);
            return mappedInterviews;
        }

        public async Task<GetInterviewDTO> CreateInterviewAsync(CreateInterviewDTO requestBody)
        {
            var user = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId).AsNoTracking()
                                                          .Select(hr => hr.Company.User)
                                                          .Where(u => u.RoleId == (int)RoleEnum.HR
                                                                   && u.Status == (int)UserStatus.Active)
                                                          .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HR_FIELD, ErrorMessage.HR_NOT_EXIST);

            var selectedDevsAndDevIds = await _unitOfWork.SelectedDevRepository
                                        .Get(s => s.RequestId == requestBody.RequestId &&
                                                  s.Status == (int)SelectedDevStatus.WaitingInterview &&
                                                  s.Request.Status == (int)HiringRequestStatus.InProgress)
                                        .Include(s => s.Developer)
                                        .Select(s => new
                                        {
                                            SelectedDev = s,
                                            DeveloperId = s.DeveloperId
                                        })
                                        .ToListAsync();

            var selectedDevs = selectedDevsAndDevIds.Select(item => item.SelectedDev).ToList();
            var devIds = selectedDevsAndDevIds.Select(item => item.DeveloperId).ToList();

            var newInterview = _mapper.Map<Interview>(requestBody);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                newInterview.InterviewCode = await GenerateUniqueCodeName();
                newInterview.Status = (int)InterviewStatus.WaitingManagerApproval;
                newInterview.CreateAt = DateTime.Now;   
                newInterview.NumOfInterviewee = devIds.Count;
                HandleDeveloperInterview(newInterview, devIds);
                HandleSelectedDevToInterviewing(selectedDevs);
                await _unitOfWork.InterviewRepository.InsertAsync(newInterview);
                await _unitOfWork.SaveChangesAsync();
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

        private void HandleDeveloperInterview(Interview interview, IEnumerable<int> devIds)
        {
            interview.DeveloperInterviews = devIds.Select(devId =>
                                                new DeveloperInterview
                                                {
                                                    InterviewId = interview.InterviewId,
                                                    DeveloperId = devId,
                                                }).ToList();
        }

        private void HandleSelectedDevToInterviewing(List<SelectedDev> selectedDevs)
        {
            selectedDevs.ForEach(d =>
            {
                d.Status = (int)SelectedDevStatus.Interviewing;
                _unitOfWork.SelectedDevRepository.Update(d);
            });
        }

        public async Task<GetInterviewDTO> ChangeStatusAsync(ChangeStatusDTO requestBody)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.InterviewId == requestBody.InterviewId &&
                                                                           i.Status == (int)InterviewStatus.WaitingManagerApproval)
                                                                 .Include(d => d.DeveloperInterviews)
                                                                 .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (requestBody.isApproved)
                {
                    var staff = await _unitOfWork.UserRepository.Get(u => u.UserId == requestBody.AssignStaffId &&
                                                                          u.RoleId == (int)RoleEnum.Staff &&
                                                                          u.Status == (int)UserStatus.Active)
                                                                .SingleOrDefaultAsync()
                       ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STAFF_FIELD, ErrorMessage.STAFF_NOT_EXIST);
                    interview.Status = (int)InterviewStatus.Interviewing;
                    interview.AssignStaffId = requestBody.AssignStaffId;
                }
                else
                {
                    interview.Status = (int)InterviewStatus.ManagerRejected;
                    interview.RejectionReason = requestBody.RejectionReason;
                }

                await UpdateStatusToInterviewingAsync(requestBody.isApproved, interview);

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

        private async Task UpdateStatusToInterviewingAsync(bool isApproved, Interview interview)
        {
            var selectedDevs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == interview.RequestId &&
                                                                                interview.DeveloperInterviews.Select(d => d.DeveloperId).Contains(s.DeveloperId))
                                                                      .Include(s => s.Developer)
                                                                             .ThenInclude(d => d.UserId)
                                                                      .ToListAsync();
            if (isApproved)
                foreach (var sd in selectedDevs)
                {
                    await _notificationService.SendNotificationAsync(sd.Developer.UserId, interview.InterviewId, NotificationTypeString.INTERVIEW,
                           $"You have been invited for an interview for request #{sd.RequestId}. Check out the interview details!");
                }
            else
                selectedDevs.ForEach(sd =>
                {
                    sd.Status = (int)SelectedDevStatus.WaitingHRAccept;

                });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalInterviewsAsync(int? companyId = null)
        {
            IQueryable<Interview> interviewQuery = _unitOfWork.InterviewRepository.Get()
                                                                                  .Include(i => i.Request);
            if (companyId.HasValue)
            {
                interviewQuery = interviewQuery.Where(i => i.Request.CompanyId == companyId.Value);
            }

            var total = await interviewQuery.CountAsync();

            return total;
        }

        public Task<int> GetTotalDevInterviewAsync(int interviewId)
        {
            var total = _unitOfWork.DeveloperInterviewRepository.Get(d => d.InterviewId == interviewId)
                                                                .CountAsync();
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
                codeName = "DEV_" + randomNumber.ToString();
                isExistInterviewCode = await _unitOfWork.InterviewRepository.AnyAsync(d => d.InterviewCode == codeName);
            } while (isExistInterviewCode);

            return codeName;
        }
    }
}
