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
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.InterviewEnum;
using static WeHire.Domain.Enums.UserEnum;
using ChangeStatusDTO = WeHire.Application.DTOs.Interview.ChangeStatusDTO;

namespace WeHire.Infrastructure.Services.InterviewServices
{
    internal class InterviewService : IInterviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InterviewService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetInterviewDetail> GetInterviewsByManager(PagingQuery query, int? companyId, SearchInterviewWithRequest searchKey)
        {
            var interviews = _unitOfWork.InterviewRepository.Get()
                                                            .Include(i => i.Interviewer)
                                                               .ThenInclude(u => u.CompanyPartners)
                                                            .AsQueryable();
            if (companyId.HasValue) interviews.Where(i => i.Interviewer.CompanyPartners.Any(c => c.CompanyId == companyId.Value));

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetInterviewDetail>>(interviews);
            return mappedInterviews;
        }

        public List<GetInterviewDetail> GetInterviewsByCompany(int? companyId, int? requestId, PagingQuery query, SearchInterviewDTO searchKey)
        {
            if (!companyId.HasValue) return new();
            var interviews = _unitOfWork.InterviewRepository.Get(i => i.Interviewer.CompanyPartners.Any(c => c.CompanyId == companyId))
                                                            .Include(i => i.Interviewer)
                                                               .ThenInclude(u => u.CompanyPartners)
                                                            .AsQueryable();

            if(requestId.HasValue) interviews = interviews.Where(i => i.RequestId == requestId);

            interviews = interviews.SearchItems(searchKey);
            interviews = interviews.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
            var mappedInterviews = _mapper.Map<List<GetInterviewDetail>>(interviews);
            return mappedInterviews;
        }

        public async Task<GetInterviewWithDev> GetInterviewById(int interviewId)
        {
            var interview = await _unitOfWork.InterviewRepository.GetByIdAsync(interviewId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            var devs = await _unitOfWork.SelectedDevRepository.Get(s => s.InterviewId == interviewId)
                                                            .Include(r => r.Developer.EmploymentType)
                                                            .Include(r => r.Developer.ScheduleType)
                                                            .Include(s => s.Developer.Level)
                                                            .Include(s => s.Developer.DeveloperTypes)
                                                                .ThenInclude(dt => dt.Type)
                                                            .Include(s => s.Developer.DeveloperSkills)
                                                                .ThenInclude(ds => ds.Skill)
                                                            .Select(s => s.Developer)
                                                            .ToListAsync();
            var mappedInterview = _mapper.Map<GetInterviewWithDev>(interview);
            mappedInterview.Developers = _mapper.Map<List<GetAllFieldDev>>(devs);
            return mappedInterview;
        }

        public async Task<List<GetInterviewDetail>> GetInterviewByRequestIdAsync(int requestId)
        {
            var interview = await _unitOfWork.InterviewRepository.Get(i => i.RequestId == requestId).AsNoTracking()
                                                                 .Include(i => i.Interviewer)
                                                                 .Include(i => i.AssignStaff)
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
            var interviews = await _unitOfWork.InterviewRepository.Get(i => requestIds.Contains((int)i.RequestId)).ToListAsync();
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

            var newInterview = _mapper.Map<Interview>(requestBody);
            newInterview.InterviewerId = user.UserId;
            newInterview.Status = (int)InterviewStatus.WaitingManagerApproval;

            await _unitOfWork.InterviewRepository.InsertAsync(newInterview);

            await _unitOfWork.SaveChangesAsync();

            var mappedInterview = _mapper.Map<GetInterviewDTO>(newInterview);
            return mappedInterview;
        }

        public async Task<GetInterviewDTO> ChangeStatusAsync(ChangeStatusDTO requestBody)
        {
            var interview = await GetInterviewByIdAsync(requestBody.InterviewId);

            if (requestBody.isApproved)
            {
                await UpdateStatusToInterviewingAsync(interview);
                interview.Status = (int)InterviewStatus.Interviewing;
            }
            else
            {
                interview.Status = (int)InterviewStatus.ManagerRejected;
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<GetInterviewDTO>(interview);
        }

        private async Task<Interview> GetInterviewByIdAsync(int interviewId)
        {
            var interview = await _unitOfWork.InterviewRepository.GetByIdAsync(interviewId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.INTERVIEW_FIELD, ErrorMessage.INTERVIEW_NOT_EXIST);
            return interview;
        }

        private async Task UpdateStatusToInterviewingAsync(Interview interview)
        {
            var selectedDevs = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == interview.RequestId)
                                                                      .ToListAsync();
    
            selectedDevs.ForEach(sd => sd.Status = (int)InterviewStatus.Interviewing);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalInterviewsAsync(int? companyId = null)
        {
            IQueryable<Interview> interviewQuery = _unitOfWork.InterviewRepository.Get()
                                                                                  .Include(i => i.Interviewer)
                                                                                  .ThenInclude(u => u.CompanyPartners);
            if (companyId.HasValue)
            {
                interviewQuery = interviewQuery.Where(i => i.Interviewer.CompanyPartners.Any(c => c.CompanyId == companyId.Value));
            }

            var total = await interviewQuery.CountAsync();
            
            return total;
        }
    }
}
