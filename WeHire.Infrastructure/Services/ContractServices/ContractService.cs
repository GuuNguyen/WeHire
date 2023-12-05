using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.ConvertDate;
using WeHire.Application.Utilities.Helper.EnumDescription;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.HiredDeveloperServices;
using WeHire.Infrastructure.Services.NotificationServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.ContractEnum;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.HiredDeveloperEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;
using static WeHire.Domain.Enums.InterviewEnum;

namespace WeHire.Infrastructure.Services.ContractServices
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;;
            _notificationService = notificationService;
        }

        public List<GetListContract> GetContractAsync(PagingQuery query, SearchContractDTO searchKey)
        {
            var contracts = _unitOfWork.ContractRepository.GetAll()
                                                          .Include(c => c.HiredDevelopers)
                                                            .ThenInclude(h => h.Project.Company.User)
                                                          .Include(c => c.HiredDevelopers)
                                                            .ThenInclude(h => h.Developer.User)
                                                          .OrderBy(c => c.CreatedAt)
                                                          .AsQueryable();

            contracts = contracts.SearchItems(searchKey);

            contracts = contracts.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedContracts = contracts
               .Select(c => new GetListContract
               {
                   ContractId = c.ContractId,
                   ContractCode = c.ContractCode,
                   DateSigned = ConvertDateTime.ConvertDateToString(c.DateSigned),
                   CompanyPartnerName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => h.Project.Company.CompanyName)
                                                         .SingleOrDefault(),
                   HumanResourceName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => $"{h.Project.Company.User.FirstName} {h.Project.Company.User.LastName}")
                                                         .SingleOrDefault(),
                   DeveloperName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => $"{h.Developer.User.FirstName} {h.Developer.User.LastName}")
                                                         .SingleOrDefault(),
                   CreatedAt = ConvertDateTime.ConvertDateToString(c.CreatedAt),
                   StatusString = EnumHelper.GetEnumDescription((ContractStatus)c.Status)
               })
               .ToList();
            return mappedContracts;
        }

        public List<GetListContract> GetContractByCompanyAsync(int companyId, PagingQuery query, SearchContractDTO searchKey)
        {
            var contracts = _unitOfWork.ContractRepository.Get(c => c.HiredDevelopers.Any(h => h.Project.CompanyId == companyId))                                                           
                                                          .Include(c => c.HiredDevelopers)
                                                            .ThenInclude(h => h.Project.Company.User)                                                          
                                                          .Include(c => c.HiredDevelopers)
                                                            .ThenInclude(h => h.Developer.User)
                                                          .OrderBy(c => c.CreatedAt)
                                                          .AsQueryable();

            contracts = contracts.SearchItems(searchKey);

            contracts = contracts.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedContracts = contracts
               .Select(c => new GetListContract
               {
                   ContractId = c.ContractId,
                   ContractCode = c.ContractCode,
                   DateSigned = ConvertDateTime.ConvertDateToString(c.DateSigned),
                   CompanyPartnerName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => h.Project.Company.CompanyName)
                                                         .SingleOrDefault(),
                   HumanResourceName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => $"{h.Project.Company.User.FirstName} {h.Project.Company.User.LastName}")
                                                         .SingleOrDefault(),
                   DeveloperName = c.HiredDevelopers.Where(h => h.ContractId == c.ContractId)
                                                         .Select(h => $"{h.Developer.User.FirstName} {h.Developer.User.LastName}")
                                                         .SingleOrDefault(),
                   CreatedAt = ConvertDateTime.ConvertDateToString(c.CreatedAt),
                   StatusString = EnumHelper.GetEnumDescription((ContractStatus)c.Status)
               })
               .ToList();
            return mappedContracts;
        }


        public async Task<GetPreContract> GetPreContractAsync(int developerId, int requestId)
        {
            var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId && d.Status == (int)DeveloperStatus.SelectedOnRequest)
                                                     .Include(d => d.User)
                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                             .Include(r => r.EmploymentType)
                                                             .Include(r => r.Company)
                                                             .ThenInclude(c => c.User)
                                                             .Include(r => r.Project)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var preContract = new GetPreContract
            {
                CompanyPartnerName = request.Company.CompanyName,
                CompanyPartPhoneNumber = request.Company.PhoneNumber,
                CompanyPartnerAddress = $"{request.Company.Address}, {request.Company.Country}",
                LegalRepresentation = $"{request.Company.User.FirstName} {request.Company.User.LastName}",
                LegalRepresentationPosition = "Human Resource",
                DeveloperName = $"{dev.User.FirstName}, {dev.User.LastName}",
                DeveloperPhoneNumber = dev.User.PhoneNumber,
                YearOfExperience = dev.YearOfExperience,
                BasicSalary = dev.AverageSalary,
                EmploymentType = request.EmploymentType.EmploymentTypeName,
                ProjectTitle = request.Project.ProjectName,
                FromDate = ConvertDateTime.ConvertDateToStringNumberThreeline(DateTime.Now.AddDays(7)),
                ToDate = ConvertDateTime.ConvertDateToStringNumberThreeline(request.Project.EndDate),
                StandardMonthlyWorkingHours = 168,
                OvertimePayMultiplier = (decimal?)1.5
            };
            return preContract;
        }

        public async Task<GetContractDetail> GetContractByDevAsync(int developerId, int projectId)
        {
            var hiredDev = await _unitOfWork.HiredDeveloperRepository.Get(h => h.DeveloperId == developerId &&
                                                                               h.ProjectId == projectId)
                                                                     .Include(h => h.Contract)
                                                                     .Include(h => h.Project)
                                                                     .ThenInclude(p => p.Company)
                                                                     .ThenInclude(c => c.User)
                                                                     .Include(h => h.Developer)
                                                                     .ThenInclude(d => d.User)
                                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper is not exist!");

            var contractDetail = new GetContractDetail
            {
                ContractId = hiredDev.ContractId,
                DateSigned = ConvertDateTime.ConvertDateToString(hiredDev.Contract.DateSigned),
                FromDate = ConvertDateTime.ConvertDateToString(hiredDev.Contract.FromDate),
                ToDate = ConvertDateTime.ConvertDateToString(hiredDev.Contract.ToDate),
                LegalRepresentation = hiredDev.Contract.LegalRepresentation,
                LegalRepresentationPosition = hiredDev.Contract.LegalRepresentationPosition,
                CompanyPartnerName = hiredDev.Project.Company.CompanyName,
                CompanyPartPhoneNumber = hiredDev.Project.Company.PhoneNumber,
                CompanyPartnerAddress = $"{hiredDev.Project.Company.Address}, {hiredDev.Project.Company.Country}",
                DeveloperName = $"{hiredDev.Developer.User.FirstName}, {hiredDev.Developer.User.LastName}",
                DeveloperPhoneNumber = hiredDev.Developer.User.PhoneNumber,
                ProjectTitle = hiredDev.Project.ProjectName,
                YearOfExperience = hiredDev.Developer.YearOfExperience,
                EmploymentType = hiredDev.Contract.EmployementType,
                StandardMonthlyWorkingHours = hiredDev.Contract.StandardMonthlyWorkingHours,
                OvertimePayMultiplier = hiredDev.Contract.OvertimePayMultiplier,
                BasicSalary = hiredDev.Contract.BasicSalary,
                StatusString = EnumHelper.GetEnumDescription((ContractStatus)hiredDev.Contract.Status),
            };

            return contractDetail;
        }

        public async Task<GetContractDetail> GetContractByIdAsync(int contractId)
        {
            var hiredDev = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contractId)
                                                                     .Include(h => h.Contract)
                                                                     .Include(h => h.Project)
                                                                     .ThenInclude(p => p.Company)
                                                                     .Include(h => h.Developer)
                                                                     .ThenInclude(d => d.User)
                                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Contract", "Contract is not exist!");

            var contractDetail = new GetContractDetail
            {
                ContractId = hiredDev.ContractId,
                DateSigned = ConvertDateTime.ConvertDateToString(hiredDev.Contract.DateSigned) ?? "",
                FromDate = ConvertDateTime.ConvertDateToString(hiredDev.Contract.FromDate),
                ToDate = ConvertDateTime.ConvertDateToString(hiredDev.Contract.ToDate),
                LegalRepresentation = hiredDev.Contract.LegalRepresentation,
                LegalRepresentationPosition = hiredDev.Contract.LegalRepresentationPosition,
                CompanyPartnerName = hiredDev.Project.Company.CompanyName,
                CompanyPartPhoneNumber = hiredDev.Project.Company.PhoneNumber,
                CompanyPartnerAddress = $"{hiredDev.Project.Company.Address}, {hiredDev.Project.Company.Country}",
                DeveloperName = $"{hiredDev.Developer.User.FirstName}, {hiredDev.Developer.User.LastName}",
                DeveloperPhoneNumber = hiredDev.Developer.User.PhoneNumber,
                ProjectTitle = hiredDev.Project.ProjectName,
                YearOfExperience = hiredDev.Developer.YearOfExperience,
                EmploymentType = hiredDev.Contract.EmployementType,
                StandardMonthlyWorkingHours = hiredDev.Contract.StandardMonthlyWorkingHours,
                OvertimePayMultiplier = hiredDev.Contract.OvertimePayMultiplier,
                BasicSalary = hiredDev.Contract.BasicSalary,
                StatusString = EnumHelper.GetEnumDescription((ContractStatus)hiredDev.Contract.Status),
            };

            return contractDetail;
        }


        public async Task<GetContractDTO> CreateContractAsync(CreateContractDTO requestBody)
        {
            if (!(requestBody.FromDate > DateTime.Now.AddDays(7)))
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, "FromDate", "The developer's start date must begin 7 days after the contract acceptance date!");
            }
            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestBody.RequestId &&
                                                                       r.Status == (int)HiringRequestStatus.InProgress)
                                                             .Include(j => j.Project)
                                                             .Include(r => r.EmploymentType)
                                                             .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Request", "Request is not exist!");

            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(s => s.RequestId == requestBody.RequestId &&
                                                                                     s.DeveloperId == requestBody.DeveloperId &&
                                                                                    (s.Status == (int)HiredDeveloperStatus.UnderConsideration ||
                                                                                     s.Status == (int)HiredDeveloperStatus.WaitingInterview ||
                                                                                     s.Status == (int)HiredDeveloperStatus.InterviewScheduled))
                                                                     .Include(s => s.Developer)
                                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SELECTING_DEV_FIELD, ErrorMessage.SELECTING_DEV_NOT_EXIST);

            var newContract = new Contract
            {
                ContractCode = await GenerateUniqueCodeName(),
                LegalRepresentation = requestBody.LegalRepresentation,
                LegalRepresentationPosition = requestBody.LegalRepresentationPosition,
                FromDate = requestBody.FromDate,
                ToDate = requestBody.ToDate,
                EmployementType = request.EmploymentType.EmploymentTypeName,
                BasicSalary = hiredDeveloper.Developer.AverageSalary,
                StandardMonthlyWorkingHours = 168,
                OvertimePayMultiplier = (decimal)1.5,
                CreatedAt = DateTime.Now,
                Status = (int)ContractStatus.Pending
            };

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                hiredDeveloper.Status = (int)HiredDeveloperStatus.ContractProcessing;
                await _unitOfWork.ContractRepository.InsertAsync(newContract);
                hiredDeveloper.Contract = newContract;
                var interviews = _unitOfWork.InterviewRepository.Get(i => i.RequestId == request.RequestId &&
                                                                         i.HiredDeveloperId == hiredDeveloper.HiredDeveloperId &&
                                                                         i.Status != (int)InterviewStatus.Cancelled &&
                                                                         i.Status != (int)InterviewStatus.Rejected)
                                                               .ToList();
                if (interviews.Any())
                {
                    foreach (var interview in interviews)
                    {
                        interview.Status = (int)InterviewStatus.Cancelled;
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _notificationService.SendNotificationAsync(hiredDeveloper.Developer.UserId, newContract.ContractId, NotificationTypeString.CONTRACT,
                               $"Contract #{newContract.ContractCode} has been created for you!");
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedContract = _mapper.Map<GetContractDTO>(newContract);
            return mappedContract;
        }


        public async Task<GetContractDTO> ConfirmSignedContractAsync(int contractId)
        {
            var contract = await _unitOfWork.ContractRepository.Get(c => c.ContractId == contractId && c.Status == (int)ContractStatus.Pending).SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.CONTRACT_FIELD, ErrorMessage.CONTRACT_NOT_EXIST);

            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contract.ContractId &&
                                                                               h.Status == (int)HiredDeveloperStatus.ContractProcessing)
                                                                     .Include(h => h.Developer)
                                                                     .Include(h => h.Project)
                                                                     .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "Hired developer does not exist!");

            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == hiredDeveloper.RequestId)
                                                                  .Include(r => r.HiredDevelopers)
                                                                  .SingleOrDefaultAsync();

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                contract.DateSigned = DateTime.Now;
                contract.Status = (int)ContractStatus.Signed;

                hiredDeveloper.Developer.Status = (int)DeveloperStatus.OnWorking;
                hiredDeveloper.Status = (int)HiredDeveloperStatus.Working;
                hiredDeveloper.Project.NumberOfDev++;

                if(request.HiredDevelopers.Count(h => h.Status == (int)HiredDeveloperStatus.Working) == request.TargetedDev)
                {
                    request.Status = (int)HiringRequestStatus.Completed;
                }
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedContract = _mapper.Map<GetContractDTO>(contract);
            return mappedContract;
        }


        public async Task<GetContractDTO> FailContractAsync(int contractId)
        {
            var contract = await _unitOfWork.ContractRepository.GetByIdAsync(contractId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.CONTRACT_FIELD, ErrorMessage.CONTRACT_NOT_EXIST);
            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contract.ContractId &&
                                                                              h.Status == (int)HiredDeveloperStatus.ContractProcessing)
                                                                    .Include(h => h.Developer)
                                                                    .Include(h => h.Project)
                                                                    .ThenInclude(p => p.Company)
                                                                    .SingleOrDefaultAsync()
             ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "Hired developer does not exist!");

            hiredDeveloper.Status = (int)HiredDeveloperStatus.ContractFailed;
            contract.Status = (int)ContractStatus.Failed;

            var hrId = hiredDeveloper.Project.Company.UserId;
            await _notificationService.SendNotificationAsync(hrId, contract.ContractId, NotificationTypeString.CONTRACT,
                                $"Contract  #{contract.ContractCode} has been failed!");

            await _unitOfWork.SaveChangesAsync();
            var mappedContract = _mapper.Map<GetContractDTO>(contract);
            return mappedContract;
        }

        public async Task FailContractOnBackgroundAsync(DateTime currentDate)
        {
            var contracts = _unitOfWork.ContractRepository.Get(c => c.Status == (int)ContractStatus.Pending &&
                                                                   currentDate > c.CreatedAt.Value.AddDays(7));
            if (contracts.Any())
            {
                foreach (var contract in contracts)
                {
                    var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contract.ContractId &&
                                                                              h.Status == (int)HiredDeveloperStatus.ContractProcessing)
                                                                    .Include(h => h.Developer)
                                                                    .Include(h => h.Project)
                                                                    .ThenInclude(p => p.Company)
                                                                    .SingleOrDefaultAsync();

                    hiredDeveloper.Status = (int)HiredDeveloperStatus.ContractFailed;
                    contract.Status = (int)ContractStatus.Failed;

                    var hrId = hiredDeveloper.Project.Company.UserId;
                    await _notificationService.SendNotificationAsync(hrId, contract.ContractId, NotificationTypeString.CONTRACT,
                                        $"Contract  #{contract.ContractCode} has been failed!");

                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }


        private async Task<string> GenerateUniqueCodeName()
        {
            Random random = new Random();
            string codeName;
            var isExistRequestCode = false;
            do
            {
                int randomNumber = random.Next(10000, 100000);
                codeName = "CONTRACT-" + randomNumber.ToString();
                isExistRequestCode = await _unitOfWork.RequestRepository.AnyAsync(d => d.RequestCode == codeName);
            } while (isExistRequestCode);
            return codeName;
        }

        public async Task<int> GetTotalItemAsync(int? companyId = null)
        {
            var total = _unitOfWork.ContractRepository.GetAll()
                  .Include(c => c.HiredDevelopers)
                  .ThenInclude(h => h.Project);
            if (companyId.HasValue)
                return await total.Where(c => c.HiredDevelopers.Any(h => h.Project.CompanyId == companyId)).CountAsync();
            return await total.CountAsync();
        }
    }
}
