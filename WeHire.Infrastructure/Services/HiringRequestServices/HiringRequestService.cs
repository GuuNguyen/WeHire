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
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.NotificationServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.LevelEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;
using static WeHire.Domain.Enums.SkillEnum;
using static WeHire.Domain.Enums.TypeEnum;

namespace WeHire.Infrastructure.Services.HiringRequestServices
{
    public class HiringRequestService : IHiringRequestService
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HiringRequestService(IUnitOfWork unitOfWork, IMapper mapper,
                                    INotificationService notificationService)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetListHiringRequest> GetAllRequest(PagingQuery query,
                                                      SearchHiringRequestDTO searchKey,
                                                      SearchExtensionDTO searchExtensionKey)
        {
            IQueryable<HiringRequest> requests = _unitOfWork.RequestRepository.GetAll()
                                                        .Include(r => r.Project)
                                                            .ThenInclude(p => p.Company)
                                                        .OrderByDescending(n => n.CreatedAt);

            requests = SearchBySkillIds(requests, searchExtensionKey.SkillIds);
            requests = SearchBySalary(requests, searchExtensionKey);
            requests = requests.SearchItems(searchKey);
            requests = requests.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedRequests = _mapper.Map<List<GetListHiringRequest>>(requests);
            return mappedRequests;
        }

        private IQueryable<HiringRequest> SearchBySalary(IQueryable<HiringRequest> query, SearchExtensionDTO searchExtensionKey)
        {
            if (searchExtensionKey.StartSalaryPerDev.HasValue)
            {
                query = query.Where(r => r.SalaryPerDev >= searchExtensionKey.StartSalaryPerDev.Value);
            }
            if (searchExtensionKey.EndSalaryPerDev.HasValue)
            {
                query = query.Where(r => r.SalaryPerDev <= searchExtensionKey.EndSalaryPerDev.Value);
            }
            return query;
        }

        private IQueryable<HiringRequest> SearchBySkillIds(IQueryable<HiringRequest> query, List<int>? skillIds)
        {
            if (skillIds != null && skillIds.Any())
            {
                foreach (var skillId in skillIds)
                {
                    query = query.Where(r => r.SkillRequires.Any(sr => sr.SkillId == skillId));
                }
            }
            return query;
        }

        public async Task<List<GetAllFieldRequest>> GetRequestsByCompanyId(int companyId,
                                                                           PagingQuery query,
                                                                           SearchHiringRequestDTO searchKey,
                                                                           SearchExtensionDTO searchExtensionKey)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId)
                   ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            IQueryable<HiringRequest> requests = _unitOfWork.RequestRepository.Get(r => r.Project.CompanyId == companyId &&
                                                                                        r.Status != (int)HiringRequestStatus.Expired)
                                                                              .Include(c => c.Project)
                                                                              .Include(r => r.EmploymentType)
                                                                              .Include(r => r.ScheduleType)
                                                                              .Include(lr => lr.LevelRequire)
                                                                              .Include(tr => tr.TypeRequire)
                                                                              .Include(sr => sr.SkillRequires)
                                                                                 .ThenInclude(s => s.Skill)
                                                                              .OrderByDescending(n => n.CreatedAt);

            requests = SearchBySkillIds(requests, searchExtensionKey.SkillIds);
            requests = SearchBySalary(requests, searchExtensionKey);
            requests = requests.SearchItems(searchKey);
            requests = requests.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedRequests = _mapper.Map<List<GetAllFieldRequest>>(requests);
            return mappedRequests;
        }

        public async Task<List<GetAllFieldRequest>> GetExpiredRequestsByCompanyId(int companyId, PagingQuery query, SearchHiringRequestDTO searchKey, SearchExtensionDTO searchExtensionKey)
        {
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId)
                  ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.COMPANY_FIELD, ErrorMessage.COMPANY_NOT_EXIST);

            IQueryable<HiringRequest> requests = _unitOfWork.RequestRepository.Get(r => r.Project.CompanyId == companyId &&
                                                                                        r.Status == (int)HiringRequestStatus.Expired)
                                                                              .Include(c => c.Project)
                                                                              .Include(r => r.EmploymentType)
                                                                              .Include(r => r.ScheduleType)
                                                                              .Include(lr => lr.LevelRequire)
                                                                              .Include(tr => tr.TypeRequire)
                                                                              .Include(sr => sr.SkillRequires)
                                                                                 .ThenInclude(s => s.Skill)
                                                                              .OrderBy(n => n.Duration);

            requests = SearchBySkillIds(requests, searchExtensionKey.SkillIds);
            requests = SearchBySalary(requests, searchExtensionKey);
            requests = requests.SearchItems(searchKey);
            requests = requests.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedRequests = _mapper.Map<List<GetAllFieldRequest>>(requests);
            return mappedRequests;
        }

        public async Task<GetAllFieldRequest> GetRequestByIdAsync(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                             .Include(r => r.Project)
                                                             .Include(r => r.EmploymentType)
                                                             .Include(r => r.ScheduleType)
                                                             .Include(r => r.TypeRequire)
                                                             .Include(r => r.LevelRequire)
                                                             .Include(r => r.SkillRequires)
                                                                  .ThenInclude(sr => sr.Skill)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            
            var mappedRequest = _mapper.Map<GetAllFieldRequest>(request);
            return mappedRequest;
        }

        public async Task<GetRequestDTO> CreateRequestAsync(CreateRequestDTO requestBody)
        {
            if ((requestBody.SkillIds == null || !requestBody.SkillIds.Any()
                || requestBody.TypeRequireId == null || requestBody.LevelRequireId == null
                || requestBody.JobTitle == null || requestBody.JobDescription == null
                || requestBody.Duration == null || requestBody.NumberOfDev == null
                || requestBody.SalaryPerDev == null) && !requestBody.isSaved)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            if (!requestBody.SkillIds.Any()
                && requestBody.TypeRequireId == null && requestBody.LevelRequireId == null
                && requestBody.JobTitle == null && requestBody.JobDescription == null
                && requestBody.Duration == null && requestBody.NumberOfDev == null
                && requestBody.SalaryPerDev == null && requestBody.isSaved)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            var companyName = await _unitOfWork.ProjectRepository.Get(c => c.ProjectId == requestBody.ProjectId)
                                                                 .Include(c => c.Company)
                                                                 .Select(c => c.Company.CompanyName)
                                                                 .SingleOrDefaultAsync();
   
            var newRequest = _mapper.Map<HiringRequest>(requestBody);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                newRequest.RequestCode = await GenerateUniqueCodeName();
                newRequest.TargetedDev = 0;
                newRequest.CreatedAt = DateTime.Now;
                newRequest.Status = requestBody.isSaved ? newRequest.Status = (int)HiringRequestStatus.Saved
                                                        : newRequest.Status = (int)HiringRequestStatus.WaitingApproval;
             
                await HandleLevels(requestBody.LevelRequireId);
                await HandleTypes(requestBody.TypeRequireId);
                await HandleSkills(newRequest, requestBody.SkillIds);

                await _unitOfWork.RequestRepository.InsertAsync(newRequest);
                await _unitOfWork.SaveChangesAsync();
                if (!requestBody.isSaved)
                {
                    await _notificationService.SendManagerNotificationAsync(companyName, newRequest.RequestId, NotificationTypeString.HIRING_REQUEST,
                       $"They are posted a new request to hire developers for their company. The request is awaiting your approval.");
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedNewRequest = _mapper.Map<GetRequestDTO>(newRequest);
            return mappedNewRequest;
        }

        public async Task<GetRequestDTO> UpdateRequestAsync(int requestId, UpdateRequestDTO requestBody)
        {
            if ((requestBody.SkillIds == null || !requestBody.SkillIds.Any()
               || requestBody.TypeRequireId == null || requestBody.LevelRequireId == null
               || requestBody.JobTitle == null || requestBody.JobDescription == null
               || requestBody.Duration == null || requestBody.NumberOfDev == null
               || requestBody.SalaryPerDev == null) && !requestBody.isSaved)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            if (!requestBody.SkillIds.Any()
                && requestBody.TypeRequireId == null && requestBody.LevelRequireId == null
                && requestBody.JobTitle == null && requestBody.JobDescription == null
                && requestBody.Duration == null && requestBody.NumberOfDev == null
                && requestBody.SalaryPerDev == null && requestBody.isSaved)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            if (requestId != requestBody.RequestId)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId).Include(r => r.SkillRequires).SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var updateRequest = _mapper.Map(requestBody, request);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await HandleSkills(updateRequest, requestBody.SkillIds);
                await HandleLevels(requestBody.LevelRequireId);
                await HandleTypes(requestBody.TypeRequireId);
                updateRequest.Status = requestBody.isSaved ? updateRequest.Status = (int)HiringRequestStatus.Saved
                                                           : updateRequest.Status = (int)HiringRequestStatus.WaitingApproval;
                _unitOfWork.RequestRepository.Update(updateRequest);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedNewRequest = _mapper.Map<GetRequestDTO>(updateRequest);
            return mappedNewRequest;
        }

        public async Task<GetRequestDTO> CloneARequestAsync(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId).AsNoTracking()
                                                            .Include(r => r.TypeRequire)
                                                            .Include(r => r.LevelRequire)
                                                            .Include(r => r.SkillRequires)
                                                                 .ThenInclude(sr => sr.Skill)
                                                            .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            await HandleLevels((int)request.TypeRequireId!);
            await HandleTypes((int)request.LevelRequireId!);
            dynamic clonedRequest;
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                clonedRequest = new HiringRequest
                {
                    JobTitle = request.JobTitle,
                    RequestCode = await GenerateUniqueCodeName(),
                    JobDescription = request.JobDescription,
                    NumberOfDev = request.NumberOfDev,
                    TargetedDev = 0,
                    SalaryPerDev = request.SalaryPerDev,
                    Duration = request.Duration,
                    CreatedAt = DateTime.Now,
                    ProjectId = request.ProjectId,
                    ScheduleTypeId = request.ScheduleTypeId,
                    EmploymentTypeId = request.EmploymentTypeId,
                    TypeRequireId = request.TypeRequireId,
                    LevelRequireId = request.LevelRequireId,
                    Status = (int)HiringRequestStatus.Saved,
                };
                var skillIds = request.SkillRequires.Select(sr => sr.SkillId).ToList();
                await HandleSkills(clonedRequest, skillIds);

                await _unitOfWork.RequestRepository.InsertAsync(clonedRequest);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedNewRequest = _mapper.Map<GetRequestDTO>(clonedRequest);
            return mappedNewRequest;
        }

        private async Task HandleTypes(int? typeId)
        {
            if (typeId == null) return;

            var isExistType = await _unitOfWork.TypeRepository.AnyAsync(t => t.TypeId == typeId &&
                                                                       t.Status == (int)TypeStatus.Active);
            if (!isExistType)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TYPE_FIELD, ErrorMessage.TYPE_NOT_EXIST);
        }

        private async Task HandleLevels(int? levelId)
        {
            if (levelId == null) return;

            var isExistLevel = await _unitOfWork.LevelRepository.AnyAsync(l => l.LevelId == levelId &&
                                                                               l.Status == (int)LevelStatus.Active);
            if (!isExistLevel)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.LEVEL_FIELD, ErrorMessage.LEVEL_NOT_EXIST);
        }

        private async Task HandleSkills(HiringRequest hiringRequest, IEnumerable<int>? skillIds)
        {
            if (skillIds == null || !skillIds.Any()) return;
            var skills = _unitOfWork.SkillRepository.Get(s => skillIds.Contains(s.SkillId))
                                                    .Where(s => s.Status == (int)SkillStatus.Active);

            if (skills.Count() != skillIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SKILL_FIELD, ErrorMessage.SKILL_NOT_EXIST);

            hiringRequest.SkillRequires.Clear();
            hiringRequest.SkillRequires = await skills
                         .Select(skill => new SkillRequire
                         {
                             Skill = skill,
                             Request = hiringRequest
                         }).ToListAsync();
        }

        public List<GetAllFieldRequest> GetRequestsByDevId(int devId, int status)
        {
            var requests = _unitOfWork.SelectedDevRepository
                                            .Get(s => s.DeveloperId == devId && s.Status == status)
                                            .Include(r => r.Request.Project)
                                            .Include(r => r.Request.EmploymentType)
                                            .Include(r => r.Request.ScheduleType)
                                            .Include(r => r.Request.LevelRequire)
                                            .Include(r => r.Request.TypeRequire)
                                            .Include(r => r.Request.SkillRequires)
                                                .ThenInclude(sr => sr.Skill)
                                            .Select(s => s.Request)
                                            .Where(r => r.Status == (int)HiringRequestStatus.InProgress)
                                            .ToList();

            var mappedRequests = _mapper.Map<List<GetAllFieldRequest>>(requests);
            return mappedRequests;
        }

        public List<EnumDetailDTO> GetRequestStatus()
        {
            var enumValues = EnumToListHelper.ConvertEnumToListValue<HiringRequestStatus>();
            return enumValues;
        }

        public async Task<int> GetTotalRequestsAsync(int? companyId = null)
        {
            IQueryable<HiringRequest> requestQuery = _unitOfWork.RequestRepository.GetAll();

            if (companyId.HasValue)
            {
                requestQuery = requestQuery.Where(r => r.Project.CompanyId == companyId.Value);
            }

            var totalItemCount = await requestQuery.CountAsync();

            return totalItemCount;
        }


        public async Task CheckRequestDeadline(DateTime currentTime)
        {
            var requests = await _unitOfWork.RequestRepository.Get(r => r.Status == (int)HiringRequestStatus.InProgress
                                                                       && currentTime > r.Duration)
                                                              .Include(r => r.Project)
                                                                   .ThenInclude(p => p.Company)
                                                              .ToListAsync();
            if (requests.Any())
            {
                foreach (var request in requests)
                {
                    var hrId = request.Project.Company.UserId;
                    request.Status = (int)HiringRequestStatus.Expired;
                    await _notificationService.SendNotificationAsync(hrId, request.RequestId, NotificationTypeString.HIRING_REQUEST,
                                         $"Your hiring request #{request.RequestId} has expired!");
                    _unitOfWork.RequestRepository.Update(request);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        public Task<int> GetTotalExpiredRequestsAsync(int companyId)
        {
             var total = _unitOfWork.RequestRepository.Get(r => r.Project.CompanyId == companyId &&
                                                                r.Status == (int)HiringRequestStatus.Expired)
                                                        .Include(r => r.Project)
                                                        .CountAsync();
            return total;
        }

        public async Task<string> DeleteHiringRequest(int requestId)
        {
            var hiringRequest = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId &&
                                                                             r.Status == (int)HiringRequestStatus.Saved)
                                                                   .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);
            if (hiringRequest.SkillRequires.Any())
            {
                hiringRequest.SkillRequires.Clear();
            }
            await _unitOfWork.RequestRepository.DeleteAsync(requestId);
            await _unitOfWork.SaveChangesAsync();
            return "Delete success!";
        }

        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistRequestCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "REQ" + randomNumber.ToString();
                isExistRequestCode = await _unitOfWork.RequestRepository.AnyAsync(d => d.RequestCode == codeName);
            } while (isExistRequestCode);
            return codeName;
        }
    }
}
