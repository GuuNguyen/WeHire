using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.AssignTask;
using WeHire.Application.DTOs.CV;
using WeHire.Application.DTOs.DeveloperTaskAssignment;
using WeHire.Application.Utilities.Helper.EnumToList;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.AssignTaskServices
{
    public interface IAssignTaskService
    {
        public List<GetAssignTaskDTO> GetAllAssignTask(PagingQuery query, SearchAssignTaskDTO searchKey);
        public List<GetAssignTaskDTO> GetAllAssignTaskByStaff(int staffId, PagingQuery query, SearchAssignTaskDTO searchKey);
        public Task<GetAssignTaskDetail> GetAssignTaskByIdAsync(int taskId);
        public Task<GetAssignTaskDTO> CreateAssignTaskAsync(CreateAssignTaskDTO requestBody);
        public Task<GetAssignTaskDTO> ApprovalTaskAsync(ApprovalStatusTask requestBody);
        public Task<GetAssignTaskDTO> FinishTaskAsync(int taskId);
        public Task<GetDevTaskAssignmentDTO> ChangeStatusDevTaskAssignmentAsync(ChangeStatusDevTaskAssignmentDTO requestBody);
        public Task<int> GetTotalItemAsync();
        public Task<int> GetTotalTaskByStaffAsync(int staffId);
        public List<EnumDetailDTO> GetTaskStatus();
    }
}
