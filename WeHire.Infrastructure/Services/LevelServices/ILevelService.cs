using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Level;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.LevelServices
{
    public interface ILevelService 
    {
        public List<GetLevelDetail> GetAllLevel(PagingQuery query, SearchLevelDTO searchKey);
        public Task<GetLevelDetail> CreateLevelAsync(CreateLevelDTO requestBody);
        public Task UpdateLevelAsync(UpdateLevelModel requestBody);
        public Task DeleteLevelAsync(int levelId);
        public Task<int> GetTotalItemAsync();
    }
}
