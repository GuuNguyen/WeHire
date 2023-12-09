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
using WeHire.Application.Services.FileServices;
using WeHire.Application.Services.PercentCalculatServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.LevelEnum;
using static WeHire.Domain.Enums.SkillEnum;
using static WeHire.Domain.Enums.TypeEnum;
using static WeHire.Domain.Enums.UserEnum;
using WeHire.Infrastructure.IRepositories;

namespace WeHire.Application.Services.DeveloperServices
{
    public class DeveloperService : IDeveloperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IPercentCalculateService _percentCalculateService;

        public DeveloperService(IUnitOfWork unitOfWork, IMapper mapper,
                                IFileService fileService, IPercentCalculateService percentCalculateService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _percentCalculateService = percentCalculateService;
        }

        public List<GetAllFieldDev> GetAllDev(PagingQuery query, SearchDeveloperDTO searchKey)
        {
            IQueryable<Developer> devs = _unitOfWork.DeveloperRepository
                                                        .GetAll()
                                                        .Include(u => u.User)
                                                            .ThenInclude(u => u.Role)
                                                        .Include(u => u.Gender)
                                                        .Include(r => r.EmploymentType)
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

        public async Task<GetDevDetail> GetDevByIdAsync(int devId)
        {
            var dev = await _unitOfWork.DeveloperRepository
                                            .Get(d => d.DeveloperId == devId).AsNoTracking()
                                            .Include(u => u.User)
                                            .Include(u => u.Gender)
                                            .Include(r => r.EmploymentType)
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

            var mappedLevel = _mapper.Map<GetLevelDeveloper>(dev.Level);
            var mappedSkills = _mapper.Map<List<GetSkillDeveloper>>(skills);
            var mappedTypes = _mapper.Map<List<GetTypeDeveloper>>(types);

            var newDevDetail = _mapper.Map<GetDevDetail>(dev);
            return newDevDetail;
        }

        public List<GetDeveloperInProject> GetDevsByProjectId(DevInProjectRequestModel requestBody)
        {
            var hiredDevs = _unitOfWork.HiredDeveloperRepository.Get(h => h.ProjectId == requestBody.ProjectId)
                                                           .Include(h => h.Developer.HiredDevelopers)
                                                           .ThenInclude(h => h.Contract)
                                                           .Include(s => s.Developer.User)
                                                           .Include(s => s.Developer.Gender)
                                                           .Include(r => r.Developer.EmploymentType)
                                                           .Include(s => s.Developer.Level)
                                                           .Include(s => s.Developer.DeveloperTypes)
                                                               .ThenInclude(dt => dt.Type)
                                                           .Include(s => s.Developer.DeveloperSkills)
                                                               .ThenInclude(ds => ds.Skill)
                                                           .ToList();

            if(requestBody.Status != null && requestBody.Status.Any())
                hiredDevs = hiredDevs.Where(h => requestBody.Status.Contains(h.Status)).ToList();

            var devs = hiredDevs.Select(h => h.Developer).ToList();

            var mappedDevs = _mapper.Map<List<GetDeveloperInProject>>(devs);
            return mappedDevs;
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
            var developerExistedInRequest = await _unitOfWork.HiredDeveloperRepository.Get(h => h.RequestId == requestId)
                                                                                      .AsNoTracking()
                                                                                      .Include(h => h.Developer)
                                                                                      .Select(h => h.Developer)
                                                                                      .ToListAsync();
            var devsExpected = GetExceptDev(devs, developerExistedInRequest);
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

        public async Task<GetDevDTO> CreateDevAsync(CreateDevDTO requestBody)
        {
            var newUser = _mapper.Map<User>(requestBody);
            var newDev = _mapper.Map<Developer>(requestBody);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await IsExistPhoneNumber(requestBody.PhoneNumber);
                await IsExistEmail(requestBody.Email);

                newUser.Password = GenerateRandomPassword(12);
                newUser.Status = (int)UserStatus.Active;
                newUser.RoleId = (int)RoleEnum.Developer;

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

        public async Task<GetDevDTO> UpdateDevProfileByAdminAsync(int developerId, UpdateDevByAdmin requestBody)
        {
            if (developerId != requestBody.DeveloperId)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "developerId", "developerId not match with each other");
            }

            var user = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId)
                                                           .Include(d => d.User)
                                                           .Select(d => d.User).SingleOrDefaultAsync();
            var developer = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId)
                                                                 .Include(d => d.DeveloperSkills)
                                                                 .Include(d => d.DeveloperTypes)
                                                                 .SingleOrDefaultAsync();

            var updatedUser = _mapper.Map(requestBody, user);
            var updatedDev = _mapper.Map(requestBody, developer);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await IsExistPhoneNumberUpdate(user.PhoneNumber, requestBody.PhoneNumber);
                await IsExistEmailUpdate(user.Email, requestBody.Email);

                updatedUser.Password = requestBody.Password;
                if (requestBody.File != null)
                    updatedUser.UserImage = await _fileService.UploadFileAsync(requestBody.File!, $"{user.FirstName}_{user.LastName}", ChildFolderName.AVATAR_FOLDER);

                await HandleLevel(requestBody.LevelId);
                await HandleSkills(updatedDev, requestBody.Skills);
                await HandleTypes(updatedDev, requestBody.Types);

                _unitOfWork.DeveloperRepository.Update(updatedDev);
                _unitOfWork.UserRepository.Update(updatedUser);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedNewDev = _mapper.Map<GetDevDTO>(updatedDev);
            return mappedNewDev;
        }

        public async Task<GetDevDTO> UpdateDevProfileAsync(int developerId, UpdateDevModel requestBody)
        {
            if (developerId != requestBody.DeveloperId)
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "developerId", "developerId not match with each other");
            }
            var user = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId)
                                                            .Include(d => d.User)
                                                            .Select(d => d.User).SingleOrDefaultAsync();
            var developer = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId)                                                             
                                                                 .SingleOrDefaultAsync();

            var updatedUser = _mapper.Map(requestBody, user);
            var updatedDev = _mapper.Map(requestBody, developer);

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                await IsExistPhoneNumberUpdate(user.PhoneNumber, requestBody.PhoneNumber);
                if (requestBody.File != null)
                    updatedUser.UserImage = await _fileService.UploadFileAsync(requestBody.File!, $"{user.FirstName}_{user.LastName}", ChildFolderName.AVATAR_FOLDER);
                _unitOfWork.DeveloperRepository.Update(updatedDev);
                _unitOfWork.UserRepository.Update(updatedUser);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedNewDev = _mapper.Map<GetDevDTO>(updatedDev);
            return mappedNewDev;
        }

        public async Task ChangStatusDeveloperAsync(ChangeStatusDeveloper requestBody)
        {
            var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == requestBody.DeveloperId &&
                                                                     d.Status == (int)DeveloperStatus.Available)
                                                           .Include(d => d.User)
                                                           .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            dev.User.Status = requestBody.UserStatus;
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task HandleLevel(int? levelId)
        {
            var isExistLevel = await _unitOfWork.LevelRepository.AnyAsync(l => l.LevelId == levelId && l.Status == (int)LevelStatus.Active);
            if (!isExistLevel)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.LEVEL_FIELD, ErrorMessage.LEVEL_NOT_EXIST);
        }

        private async Task HandleSkills(Developer developer, IEnumerable<int> skillIds)
        {
            var skills = _unitOfWork.SkillRepository.Get(s => skillIds.Contains(s.SkillId))
                                                    .Where(t => t.Status == (int)SkillStatus.Active);

            if (skills.Count() != skillIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "skillId", "skillId count not match!");

            developer.DeveloperSkills.Clear();

            developer.DeveloperSkills = await skills
                .Where(skill => skill.Status == (int)SkillStatus.Active)
                .Select(skill => new DeveloperSkill
                {
                    Skill = skill,
                    Developer = developer
                }).ToListAsync();
        }

        private async Task HandleTypes(Developer developer, IEnumerable<int> typeIds)
        {
            var types = _unitOfWork.TypeRepository.Get(t => typeIds.Contains(t.TypeId))
                                                  .Where(t => t.Status == (int)TypeStatus.Active);

            if (types.Count() != typeIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "typeId", "typeId count not match!");

            developer.DeveloperTypes.Clear();

            developer.DeveloperTypes = await types
                .Where(type => type.Status == (int)TypeStatus.Active)
                .Select(type => new DeveloperType
                {
                    Type = type,
                    Developer = developer
                }).ToListAsync();
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.DeveloperRepository.GetAll().CountAsync();
            return total;
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

        private async Task IsExistPhoneNumberUpdate(string? oldPhoneNumber, string newPhoneNumber)
        {
            var isExist = await _unitOfWork.UserRepository.AnyAsync(u => u.PhoneNumber.Equals(newPhoneNumber) && oldPhoneNumber != newPhoneNumber);
            if (isExist)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.PHONE_NUMBER_FIELD, ErrorMessage.PHONE_NUMBER_ALREADY_EXIST);
        }

        private async Task IsExistEmailUpdate(string oldEmail, string newEmail)
        {
            var isExist = await _unitOfWork.UserRepository.AnyAsync(u => u.Email.Equals(newEmail) && !oldEmail.Equals(newEmail));
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
                codeName = "DEV" + randomNumber.ToString();
                isExistDevCode = await _unitOfWork.DeveloperRepository.AnyAsync(d => d.CodeName == codeName);
            } while (isExistDevCode);
            return codeName;
        }

        
    }
}
