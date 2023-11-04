using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.DTOs.Type;
using WeHire.Application.DTOs.User;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.FileServices;
using WeHire.Infrastructure.Services.PercentCalculatServices;
using WeHire.Infrastructure.Services.SelectingDevServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.SelectedDevEnum;
using static WeHire.Domain.Enums.SkillEnum;
using static WeHire.Domain.Enums.TypeEnum;
using static WeHire.Domain.Enums.UserEnum;

namespace WeHire.Infrastructure.Services.DeveloperServices
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ISelectingDevService _selectingDevService;
        private readonly IPercentCalculateService _percentCalculateService;

        public DeveloperService(IUnitOfWork unitOfWork, IMapper mapper,
                                IFileService fileService, IPercentCalculateService percentCalculateService,
                                ISelectingDevService selectingDevService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _selectingDevService = selectingDevService;
            _percentCalculateService = percentCalculateService;
        }

        public List<GetAllFieldDev> GetAllDev(PagingQuery query, SearchDeveloperDTO searchKey)
        {
            IQueryable<Developer> devs = _unitOfWork.DeveloperRepository
                                                        .GetAll()
                                                        .Include(u => u.User)
                                                            .ThenInclude(u => u.Role)
                                                        .Include(c => c.Cvs)
                                                        .Include(u => u.Gender)
                                                        .Include(r => r.EmploymentType)
                                                        .Include(r => r.ScheduleType)
                                                        .Include(l => l.Level)
                                                        .Include(ds => ds.DeveloperSkills)
                                                            .ThenInclude(s => s.Skill)
                                                        .Include(dt => dt.DeveloperTypes)
                                                            .ThenInclude(t => t.Type);

            devs = devs.SearchItems(searchKey);

            devs = devs.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedDev = _mapper.Map<List<GetAllFieldDev>>(devs);

            return mappedDev;
        }

        public List<GetDevDTO> GetUnofficialDev(PagingQuery query)
        {
            var unofficialDevs = _unitOfWork.DeveloperRepository.Get(d => d.Status == (int)DeveloperStatus.Available &&
                                                                          d.User.RoleId == (int)RoleEnum.Unofficial && d.User.Status == (int)UserStatus.Active)
                                                                .Include(d => d.User).AsQueryable();

            unofficialDevs = unofficialDevs.PagedItems(query.PageIndex, query.PageSize).AsQueryable();
                
            var mappedList = _mapper.Map<List<GetDevDTO>>(unofficialDevs.ToList());
            return mappedList;
        }

        public async Task<GetDevDetail> GetDevByIdAsync(int devId)
        {
            var dev = await _unitOfWork.DeveloperRepository
                                            .Get(d => d.DeveloperId == devId).AsNoTracking()
                                            .Include(u => u.User)
                                            .Include(u => u.Gender)
                                            .Include(r => r.EmploymentType)
                                            .Include(r => r.ScheduleType)
                                            .Include(c => c.Cvs)
                                            .Include(l => l.Level)
                                            .Include(ds => ds.DeveloperSkills)
                                                .ThenInclude(s => s.Skill)
                                            .Include(dt => dt.DeveloperTypes)
                                                .ThenInclude(t => t.Type)
                                            .FirstOrDefaultAsync();
            if (dev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            var skills = dev.DeveloperSkills
                                .Select(ds => ds.Skill)
                                .Where(s => s.Status == (int)SkillStatus.Active)
                                .ToList();
            var types = dev.DeveloperTypes
                                .Select(dt => dt.Type)
                                .Where(s => s.Status == (int)TypeStatus.Active)
                                .ToList();

            var mappedLevel = _mapper.Map<GetLevelDetail>(dev.Level);
            var mappedSkills = _mapper.Map<List<GetSkillDetail>>(skills);
            var mappedTypes = _mapper.Map<List<GetTypeDetail>>(types);

            var newDevDetail = _mapper.Map<GetDevDetail>(dev);
            return newDevDetail;
        }

        public async Task<List<GetAllFieldDev>> GetAllDevByTaskIdAsync(int taskId)
        {
            var devs = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId == taskId).AsNoTracking()
                                                                          .Include(dt => dt.Developer.User)
                                                                                .ThenInclude(u => u.Role)
                                                                          .Include(dt => dt.Developer.Gender)
                                                                          .Include(r => r.Developer.EmploymentType)
                                                                          .Include(r => r.Developer.ScheduleType)
                                                                          .Include(d => d.Developer.Cvs)
                                                                          .Include(d => d.Developer.Level)
                                                                          .Include(d => d.Developer.DeveloperSkills)
                                                                                .ThenInclude(ds => ds.Skill)
                                                                          .Include(d => d.Developer.DeveloperTypes)
                                                                                .ThenInclude(dty => dty.Type)
                                                                          .Select(d => d.Developer)
                                                                          .ToListAsync();
            if (devs.Count == 0)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_TASK_FIELD, ErrorMessage.DEV_TASK_NOT_EXIST);
            var mappedDev = _mapper.Map<List<GetAllFieldDev>>(devs);

            return mappedDev;
        }

        public async Task<GetDevDTO> CreateDevAsync(CreateDevDTO requestBody)
        {
            if (requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);

            var newUser = _mapper.Map<User>(requestBody);
            var newDev = _mapper.Map<Developer>(requestBody);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await IsExistPhoneNumber(requestBody.PhoneNumber);
                await IsExistEmail(requestBody.Email);

                newUser.Password = GenerateRandomPassword(12);
                newUser.Status = (int)UserStatus.Active;
                newUser.RoleId = (int)RoleEnum.Unofficial;

                newDev.Status = (int)DeveloperStatus.Available;
                newDev.CodeName = await GenerateUniqueCodeName();

                await HandleLevel(requestBody.LevelId);
                await HandleSkills(newDev, requestBody.Skills);
                await HandleTypes(newDev, requestBody.Types);

                newDev.User = newUser;

                await _unitOfWork.DeveloperRepository.InsertAsync(newDev);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedNewDev = _mapper.Map<GetDevDTO>(newDev);
            return mappedNewDev;
        }

        private async Task HandleLevel(int? levelId)
        {
            var isExistLevel = await _unitOfWork.LevelRepository.AnyAsync(l => l.LevelId == levelId);
            if (!isExistLevel)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.LEVEL_FIELD, ErrorMessage.LEVEL_NOT_EXIST);
        }

        private async Task HandleSkills(Developer developer, IEnumerable<int> skillIds)
        {
            if (skillIds.Any())
            {
                var skills = _unitOfWork.SkillRepository.Get(s => skillIds.Contains(s.SkillId))
                                                        .Where(t => t.Status == (int)SkillStatus.Active);
                if(skills.Count() != skillIds.Count())
                    throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SKILL_FIELD, ErrorMessage.SKILL_NOT_EXIST);

                developer.DeveloperSkills = await skills
                    .Where(skill => skill.Status == (int)SkillStatus.Active)
                    .Select(skill => new DeveloperSkill
                    {
                        Skill = skill,
                        Developer = developer
                    }).ToListAsync();
            }
        }

        private async Task HandleTypes(Developer developer, IEnumerable<int> typeIds)
        {
            if (typeIds.Any())
            {
                var types = _unitOfWork.TypeRepository.Get(t => typeIds.Contains(t.TypeId))
                                                      .Where(t => t.Status == (int)TypeStatus.Active);
                if (types.Count() != typeIds.Count())
                    throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TYPE_FIELD, ErrorMessage.TYPE_NOT_EXIST);

                developer.DeveloperTypes = await types
                    .Select(type => new DeveloperType
                    {
                        Type = type,
                        Developer = developer
                    }).ToListAsync();
            }
        }

        public async Task<GetDevDTO> ActiveDeveloperAsync(int developerId)
        {
            var dev = await _unitOfWork.DeveloperRepository.GetByIdAsync(developerId);

            if (dev == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            dev.Status = (int)DeveloperStatus.Available;
            _unitOfWork.DeveloperRepository.Update(dev);
            await _unitOfWork.SaveChangesAsync();

            var mappedDev = _mapper.Map<GetDevDTO>(dev);
            return mappedDev;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.DeveloperRepository.GetAll().CountAsync();
            return total;
        }

        public async Task<List<GetMatchingDev>> GetDevMatchingWithRequest(int requestId)
        {
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId && r.Status != (int)HiringRequestStatus.Saved)
                                                             .AsNoTracking()
                                                             .Include(r => r.SkillRequires)
                                                             .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var devs = _unitOfWork.DeveloperRepository.GetAll()
                                                      .Where(d => d.Status == (int)DeveloperStatus.Available
                                                                && d.User.RoleId == (int)RoleEnum.Developer
                                                                && d.User.Status == (int)UserStatus.Active)
                                                      .Include(d => d.User)
                                                      .Include(d => d.Level)
                                                      .Include(d => d.DeveloperTypes)
                                                          .ThenInclude(dt => dt.Type)
                                                      .Include(d => d.DeveloperSkills)
                                                          .ThenInclude(ds => ds.Skill)
                                                      .ToList();

            var matchingDevs = new List<GetMatchingDev>();
            var selectedDev = await _selectingDevService.GetSelectedDevsById(requestId);
            var devsExpected = GetExceptDev(devs, selectedDev);
            foreach (var dev in devsExpected)
            {
                var matchingPercentObj = _percentCalculateService.CalculateMatchingPercentage(request, dev);
                var mappedDev = _mapper.Map<GetMatchingDev>(dev);
                _mapper.Map(matchingPercentObj, mappedDev);
                matchingDevs.Add(mappedDev);
            }
            var devTaken = matchingDevs.OrderByDescending(matchedDev => matchedDev.AveragedPercentage)
                                       .Take((int)(request.NumberOfDev! * 2))
                                       .ToList();
            return devTaken;
        }

        public List<Developer> GetExceptDev(List<Developer> devs1, List<Developer> devs2)
        {
            var resultDevs = devs1
                            .Where(dev1 => !devs2.Any(dev2 => dev2.DeveloperId == dev1.DeveloperId))
                            .ToList();

            return resultDevs;
        }


        public static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                return new string(randomBytes.Select(b => validChars[b % validChars.Length]).ToArray());
            }
        }
        private async Task IsExistPhoneNumber(string? phoneNumber)
        {
            var isExist = await _unitOfWork.UserRepository.AnyAsync(u => u.PhoneNumber.Equals(phoneNumber));
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PHONE_NUMBER_FIELD, ErrorMessage.PHONE_NUMBER_ALREADY_EXIST);
        }

        private async Task IsExistEmail(string? email)
        {
            var isExist = await _unitOfWork.UserRepository.AnyAsync(u => u.Email.Equals(email));
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.EMAIL_FIELD, ErrorMessage.EMAIL_ALREADY_EXIST);
        }

        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistDevCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "DEV_" + randomNumber.ToString();
                isExistDevCode = await _unitOfWork.DeveloperRepository.AnyAsync(d => d.CodeName == codeName);
            } while (isExistDevCode);

            return codeName;
        }

        public Task<GetDevDTO> UpdateDevProfileAsync(int id, UpdateDevProfile requestBody)
        {
            throw new NotImplementedException();
        }

        public List<GetAllFieldDev> GetDevsWaitingInterview(PagingQuery query, int requestId)
        {
            var devs = _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                  s.Status == (int)SelectedDevStatus.WaitingInterview)
                                                        .Include(r => r.Developer.EmploymentType)
                                                        .Include(r => r.Developer.ScheduleType)
                                                        .Include(s => s.Developer.Level)
                                                        .Include(s => s.Developer.DeveloperTypes)
                                                            .ThenInclude(dt => dt.Type)
                                                        .Include(s => s.Developer.DeveloperSkills)
                                                            .ThenInclude(ds => ds.Skill)
                                                        .Select(s => s.Developer);

            devs = devs.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedDevs = _mapper.Map<List<GetAllFieldDev>>(devs);        
            return mappedDevs;
        }

        public async Task<int> GetTotalDevWaitingInterviewAsync(int requestId)
        {
            var total = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestId &&
                                                                 s.Status == (int)SelectedDevStatus.WaitingInterview)
                                                               .CountAsync();
            return total;
        }

        public async Task<int> GetTotalUnofficialAsync()
        {
            var total = await _unitOfWork.DeveloperRepository.Get(d => d.Status == (int)DeveloperStatus.Available &&
                                                                         d.User.RoleId == (int)RoleEnum.Unofficial && d.User.Status == (int)UserStatus.Active)
                                                               .Include(d => d.User).CountAsync();
            return total;
        }
    }
}
