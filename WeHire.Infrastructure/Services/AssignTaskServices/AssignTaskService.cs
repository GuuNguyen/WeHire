using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.AssignTask;
using WeHire.Application.DTOs.DeveloperTaskAssignment;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.AssignTaskEnum;
using static WeHire.Domain.Enums.DeveloperEnum;
using static WeHire.Domain.Enums.DeveloperTaskEnum;
using static WeHire.Domain.Enums.HiringRequestEnum;

namespace WeHire.Infrastructure.Services.AssignTaskServices
{
    public class AssignTaskService : IAssignTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AssignTaskService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public List<GetAssignTaskDTO> GetAllAssignTask(PagingQuery query, SearchAssignTaskDTO searchKey)
        {
            var assignTasks = _unitOfWork.AssignTaskRepository.GetAll();

            assignTasks = assignTasks.SearchItems(searchKey);

            assignTasks = assignTasks.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedAssignTasks = _mapper.Map<List<GetAssignTaskDTO>>(assignTasks);
            return mappedAssignTasks;
        }

        public async Task<GetAssignTaskDetail> GetAssignTaskByIdAsync(int taskId)
        {
            var task = await _unitOfWork.AssignTaskRepository.Get(t => t.TaskId ==taskId)
                                                             .Include(t => t.User)
                                                             .FirstOrDefaultAsync()
                 ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TASK_FIELD, ErrorMessage.TASK_NOT_EXIST);

            var mappedTask = _mapper.Map<GetAssignTaskDetail>(task);
            return mappedTask;
        }

        public async Task<GetAssignTaskDTO> CreateAssignTaskAsync(CreateAssignTaskDTO requestBody)
        {
            var staff = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.UserId == requestBody.UserId
                                                                                  && u.RoleId == (int)RoleEnum.Staff
                                                                                  && u.Status == (int)UserEnum.UserStatus.Active)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.STAFF_FIELD, ErrorMessage.STAFF_NOT_EXIST);
            var newAssignTask = _mapper.Map<AssignTask>(requestBody);
            
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                newAssignTask.Status = (int)AssignTaskStatus.Preparing;
                staff.Status = (int)UserEnum.UserStatus.OnTasking;

                await HandleDeveloperAssignTaskAsync(newAssignTask, requestBody.DevIds);

                _unitOfWork.UserRepository.Update(staff);
                await _unitOfWork.AssignTaskRepository.InsertAsync(newAssignTask);
                await _unitOfWork.SaveChangesAsync();

                transaction.Commit();             
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            var mappedAssignTask = _mapper.Map<GetAssignTaskDTO>(newAssignTask);
            return mappedAssignTask;
        }

        private async Task HandleDeveloperAssignTaskAsync(AssignTask assignTask, IEnumerable<int> devIds)
        {
            if (devIds == null || !devIds.Any())
            {
                return;
            }

            var validDevIds =  _unitOfWork.DeveloperRepository
                                            .Get(d => devIds.Contains(d.DeveloperId)
                                                && d.Status == (int)DeveloperStatus.Available
                                                && d.User.RoleId == (int)RoleEnum.Developer);

            if (validDevIds.Count() != devIds.Count())
            {
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);
            }

            var devs =  _unitOfWork.DeveloperRepository.Get(d => devIds.Contains(d.DeveloperId));

            var assignments = await devs.Select(dev => new DeveloperTaskAssignment
                                        {
                                            Task = assignTask,
                                            Developer = dev,
                                            Status = (int)DeveloperTaskStatus.Preparing
                                        }).ToListAsync();
            await _unitOfWork.DeveloperTaskAssignmentRepository.InsertRangeAsync(assignments);
        }


        public Task<int> GetTotalItemAsync()
        {
            var total = _unitOfWork.AssignTaskRepository.GetAll().CountAsync();
            return total;
        }

        public async Task<GetAssignTaskDTO> ChangeStatusTaskAsync(ChangeStatusTaskDTO requestBody)
        {
            var task = await _unitOfWork.AssignTaskRepository.GetByIdAsync(requestBody.TaskId)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TASK_FIELD, ErrorMessage.TASK_NOT_EXIST);

            task.Status = Enum.IsDefined(typeof(AssignTaskStatus), requestBody.Status) ? requestBody.Status
                                                                                       : throw new ExceptionResponse(HttpStatusCode.BadRequest, 
                                                                                       ErrorField.STATUS_FIELD, ErrorMessage.STATUS_NOT_EXIST);

            var mappedTask = _mapper.Map<GetAssignTaskDTO>(task);
            return mappedTask;
        }

        public async Task<GetDevTaskAssignmentDTO> ChangeStatusDevTaskAssignmentAsync(ChangeStatusDevTaskAssignmentDTO requestBody)
        {
            var devTaskAssign = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId == requestBody.TaskId
                                                                                        && dt.DeveloperId == requestBody.DeveloperId)
                                                                                   .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_TASK_FIELD, ErrorMessage.DEV_TASK_NOT_EXIST);

            devTaskAssign.Status = Enum.IsDefined(typeof(DeveloperTaskStatus), requestBody.Status) ? requestBody.Status
                                                                                       : throw new ExceptionResponse(HttpStatusCode.BadRequest,
                                                                                       ErrorField.STATUS_FIELD, ErrorMessage.STATUS_NOT_EXIST);
            var mappedDevTaskAssign = _mapper.Map<GetDevTaskAssignmentDTO>(devTaskAssign);
            return mappedDevTaskAssign;
        }

        public List<EnumDetailDTO> GetTaskStatus()
        {
            var enumValues = EnumToListHelper.ConvertEnumToListValue<AssignTaskStatus>();
            return enumValues;
        }

        
    }
}
