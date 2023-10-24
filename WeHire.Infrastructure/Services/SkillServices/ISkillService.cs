using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Domain.Entities;

namespace WeHire.Infrastructure.Services.SkillServices
{
    public interface ISkillService
    {
        public List<GetSkillDetail> GetAllSkill(PagingQuery query, SearchSkillDTO searchKey);
        public Task<GetSkillDetail> CreateSkillAsync(CreateSkillDTO requestBody);
        public Task<int> GetTotalItemAsync();
    }
}
