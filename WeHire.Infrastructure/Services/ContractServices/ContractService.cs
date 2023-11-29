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
using static WeHire.Domain.Enums.SelectedDevEnum;

namespace WeHire.Infrastructure.Services.ContractServices
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHiredDeveloperService _hiredDeveloperService;
        private readonly INotificationService _notificationService;

        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, IHiredDeveloperService hiredDeveloperService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hiredDeveloperService = hiredDeveloperService;
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


        public async Task<GetPreContract> GetPreContractAsync(int developerId, int requestId)
        {
            var dev = await _unitOfWork.DeveloperRepository.Get(d => d.DeveloperId == developerId && d.Status == (int)DeveloperStatus.SelectedOnRequest)
                                                     .Include(d => d.User)
                                                     .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);

            var request = await _unitOfWork.RequestRepository.Get(r => r.RequestId == requestId)
                                                             .Include(r => r.EmploymentType)
                                                             .Include(r => r.JobPosition)
                                                                 .ThenInclude(j => j.Project)
                                                             .Include(r => r.Company)
                                                             .ThenInclude(c => c.User)
                                                             .SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.HIRING_REQUEST_FIELD, ErrorMessage.HIRING_REQUEST_NOT_EXIST);

            var preContract = new GetPreContract
            {
                CompanyPartnerName = request.JobPosition.Project.Company.CompanyName,
                CompanyPartPhoneNumber = request.JobPosition.Project.Company.PhoneNumber,
                CompanyPartnerAddress = $"{request.JobPosition.Project.Company.Address}, {request.JobPosition.Project.Company.Country}",
                LegalRepresentation = $"{request.JobPosition.Project.Company.User.FirstName} {request.JobPosition.Project.Company.User.LastName}",
                LegalRepresentationPosition = "Human Resource",
                DeveloperName = $"{dev.User.FirstName}, {dev.User.LastName}",
                DeveloperPhoneNumber = dev.User.PhoneNumber,
                DeveloperJobPositon = request.JobPosition.PositionName,
                YearOfExperience = dev.YearOfExperience,
                BasicSalary = dev.AverageSalary,
                EmploymentType = request.EmploymentType.EmploymentTypeName,
                ProjectTitle = request.JobPosition.Project.ProjectName,
                FromDate = ConvertDateTime.ConvertDateToStringNumberThreeline(request.JobPosition.Project.StartDate),
                ToDate = ConvertDateTime.ConvertDateToStringNumberThreeline(request.JobPosition.Project.EndDate),
                StandardMonthlyWorkingHours = 168,
                OvertimePayMultiplier = (decimal?)1.5
            };
            return preContract;
        }

        public async Task<GetContractDetail> GetContractByDevAsync(int developerId, int projectId)
        {
            var hiredDev = await _unitOfWork.HiredDeveloperRepository.Get(h => h.DeveloperId == developerId &&
                                                                               h.ProjectId == projectId)
                                                                     .Include(h => h.JobPosition)
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
                DeveloperJobPositon = hiredDev.JobPosition.PositionName,
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
                                                                     .Include(h => h.JobPosition)
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
                DeveloperJobPositon = hiredDev.JobPosition.PositionName,
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
                                                             .Include(r => r.JobPosition)
                                                                  .ThenInclude(j => j.Project)
                                                             .Include(r => r.EmploymentType)
                                                             .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Request", "Request is not exist!");

            var selectingDev = await _unitOfWork.SelectedDevRepository.Get(s => s.RequestId == requestBody.RequestId &&
                                                                               s.DeveloperId == requestBody.DeveloperId &&
                                                                               s.Status == (int)SelectedDevStatus.WaitingInterview)
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
                BasicSalary = selectingDev.Developer.AverageSalary,
                StandardMonthlyWorkingHours = 168,
                OvertimePayMultiplier = (decimal)1.5,
                CreatedAt = DateTime.Now,
                Status = (int)ContractStatus.Pending
            };

            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                selectingDev.Status = (int)SelectedDevStatus.ContractProcessing;
                await _unitOfWork.ContractRepository.InsertAsync(newContract);
                await _unitOfWork.SaveChangesAsync();
                await _hiredDeveloperService.CreateHiredDeveloper(request.JobPositionId, request.JobPosition.ProjectId, selectingDev.Developer, newContract.ContractId, request.JobPosition.Project.ProjectCode);
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

            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contractId).SingleOrDefaultAsync()
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "HiredDeveloper", "HiredDeveloper not exist!");

            var request = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == hiredDeveloper.DeveloperId &&
                                                                           s.Request.JobPositionId == hiredDeveloper.JobPositionId &&
                                                                           s.Request.Status == (int)HiringRequestStatus.InProgress &&
                                                                           s.Developer.Status == (int)DeveloperStatus.SelectedOnRequest)
                                                                     .Include(s => s.Request)
                                                                     .ThenInclude(s => s.SelectedDevs)
                                                                     .ThenInclude(sd => sd.Developer)
                                                                     .Select(s => s.Request)
                                                                     .SingleOrDefaultAsync()
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, "Hiring Request", "Hiring request does not exist!");

            var selectedDev = request.SelectedDevs.SingleOrDefault(s => s.DeveloperId == hiredDeveloper.DeveloperId &&
                                                                        s.Status == (int)SelectedDevStatus.ContractProcessing);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                contract.DateSigned = DateTime.Now;
                contract.Status = (int)ContractStatus.Signed;

                selectedDev.Developer.Status = (int)DeveloperStatus.OnWorking;
                hiredDeveloper.Status = (int)HiredDeveloperStatus.Working;
                selectedDev.Status = (int)SelectedDevStatus.OnBoarding;

                if (!request.SelectedDevs.Any(s => s.Status == (int)SelectedDevStatus.WaitingInterview ||
                                                   s.Status == (int)SelectedDevStatus.InterviewScheduled ||
                                                   s.Status == (int)SelectedDevStatus.UnderConsideration ||
                                                   s.Status == (int)SelectedDevStatus.ContractProcessing))
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
            var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contract.ContractId)
                                                                             .Include(h => h.Project)
                                                                             .ThenInclude(p => p.Company)
                                                                             .SingleOrDefaultAsync();
            var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == hiredDeveloper.DeveloperId &&
                                                                               s.Status == (int)SelectedDevStatus.ContractProcessing)
                                                                     .SingleOrDefaultAsync();
            selectedDev.Status = (int)SelectedDevStatus.ContractFailed;
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
                    var hiredDeveloper = await _unitOfWork.HiredDeveloperRepository.Get(h => h.ContractId == contract.ContractId)
                                                                             .Include(h => h.Project)
                                                                             .ThenInclude(p => p.Company)
                                                                             .SingleOrDefaultAsync();
                    var selectedDev = await _unitOfWork.SelectedDevRepository.Get(s => s.DeveloperId == hiredDeveloper.DeveloperId &&
                                                                               s.Status == (int)SelectedDevStatus.ContractProcessing)
                                                                     .SingleOrDefaultAsync();
                    selectedDev.Status = (int)SelectedDevStatus.ContractFailed;
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
                codeName = "#CONTRACT-" + randomNumber.ToString();
                isExistRequestCode = await _unitOfWork.RequestRepository.AnyAsync(d => d.RequestCode == codeName);
            } while (isExistRequestCode);
            return codeName;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.ContractRepository.GetAll().CountAsync();
            return total;
        }
    }
}
