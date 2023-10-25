using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.AssignTask;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.DeveloperTaskAssignment;
using WeHire.Application.DTOs.HiringRequest;
using WeHire.Application.DTOs.User;
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
            var devs = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId ==  taskId)
                                                                          .Include(dt => dt.Developer)
                                                                              .ThenInclude(d => d.User)
                                                                          .Select(dt => dt.Developer)
                                                                          .ToListAsync();
            var staff = await _unitOfWork.UserRepository.GetByIdAsync(task.UserId!);

            var mappedTask = _mapper.Map<GetAssignTaskDetail>(task);
            mappedTask.Staff = _mapper.Map<GetUserDetail>(staff);
            mappedTask.Devs = _mapper.Map<List<GetDevDTO>>(devs);
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
                await HandleDeveloperOnTaskStatusAsync(requestBody.DevIds);

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
            if (devIds == null || !devIds.Any()) return;

            var validDevIds =  _unitOfWork.DeveloperRepository.Get(d => devIds.Contains(d.DeveloperId)
                                                && d.Status == (int)DeveloperStatus.Available
                                                && d.User.RoleId == (int)RoleEnum.Unofficial);

            if (validDevIds.Count() != devIds.Count())
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_COUNT);

            var devs =  _unitOfWork.DeveloperRepository.Get(d => devIds.Contains(d.DeveloperId));

            var assignments = await devs.Select(dev => new DeveloperTaskAssignment
                                        {
                                            Task = assignTask,
                                            Developer = dev,
                                            Status = (int)DeveloperTaskStatus.Preparing
                                        }).ToListAsync();
            await _unitOfWork.DeveloperTaskAssignmentRepository.InsertRangeAsync(assignments);
        }

        private async Task HandleDeveloperOnTaskStatusAsync(IEnumerable<int> devIds)
        {
            if (devIds == null || !devIds.Any()) return;

            var devs = _unitOfWork.DeveloperRepository.Get(d => devIds.Contains(d.DeveloperId));
            await devs.ForEachAsync(d => d.Status = (int)DeveloperStatus.OnTasking);
            await _unitOfWork.SaveChangesAsync();
        }


        public Task<int> GetTotalItemAsync()
        {
            var total = _unitOfWork.AssignTaskRepository.GetAll().CountAsync();
            return total;
        }

        public async Task<GetAssignTaskDTO> ApprovalTaskAsync(ApprovalStatusTask requestBody)
        {
            var task = await _unitOfWork.AssignTaskRepository.GetFirstOrDefaultAsync(t => t.TaskId == requestBody.TaskId && t.Status == (int)AssignTaskStatus.Preparing)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TASK_FIELD, ErrorMessage.TASK_NOT_EXIST);
            var devs = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId == requestBody.TaskId)
                                                                          .Include(dt => dt.Developer)
                                                                          .Select(dt => dt.Developer)
                                                                          .ToListAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, ErrorMessage.DEV_NOT_EXIST);
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                if (requestBody.IsApproval) task.Status = (int)AssignTaskStatus.InProgress;
                else
                {
                    task.Status = (int)AssignTaskStatus.Cancelled;
                    devs.ForEach(d =>
                    {
                        d.Status = (int)DeveloperStatus.Available;
                        _unitOfWork.DeveloperRepository.Update(d);
                    });
                }
                _unitOfWork.AssignTaskRepository.Update(task);
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedTask = _mapper.Map<GetAssignTaskDTO>(task);
            return mappedTask;
        }

        public async Task<GetAssignTaskDTO> FinishTaskAsync(int taskId)
        {
            var task = await _unitOfWork.AssignTaskRepository.GetFirstOrDefaultAsync(t => t.TaskId == taskId && t.Status == (int)AssignTaskStatus.InProgress)
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TASK_FIELD, ErrorMessage.TASK_NOT_EXIST);
            var devTasks = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId == taskId)
                                                                              .Include(dt => dt.Developer)
                                                                              .ToListAsync();
            devTasks.ForEach(d =>
            {
                if(d.Status != (int)DeveloperTaskStatus.Success)
                   throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_FIELD, $"Developer have code = {d.Developer.CodeName} is not finished yet!");
            });
            var devs = devTasks.Select(dt => dt.Developer).ToList();
            using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                devs.ForEach(d =>
                {
                    d.Status = (int)DeveloperStatus.Available;
                    _unitOfWork.DeveloperRepository.Update(d);
                });
                task.Status = (int)AssignTaskStatus.Done;
                await _unitOfWork.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }

            var mappedTask = _mapper.Map<GetAssignTaskDTO>(task);
            return mappedTask;
        }

        public async Task<GetDevTaskAssignmentDTO> ChangeStatusDevTaskAssignmentAsync(ChangeStatusDevTaskAssignmentDTO requestBody)
        {
            var devTaskAssign = await _unitOfWork.DeveloperTaskAssignmentRepository.Get(dt => dt.TaskId == requestBody.TaskId
                                                                                        && dt.DeveloperId == requestBody.DeveloperId)
                                                                                   .SingleOrDefaultAsync()
                ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.DEV_TASK_FIELD, ErrorMessage.DEV_TASK_NOT_EXIST);

            devTaskAssign.Status = Enum.IsDefined(typeof(DeveloperTaskStatus), requestBody.Status) && requestBody.Status != (int)DeveloperTaskStatus.Preparing 
                                                                                       ? requestBody.Status
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
