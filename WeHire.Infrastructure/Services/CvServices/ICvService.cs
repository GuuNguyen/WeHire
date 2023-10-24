using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.CV;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Infrastructure.Services.CvServices
{
    public interface ICvService
    {
        public List<GetCvDetail> GetAllCv(PagingQuery query, SearchCvDTO searchKey);
        public Task<GetCvDetail> GetCvByIdAsync(int id);
        public Task<GetCvDetail> CreateCvAsync(CreateCvDTO requestBody);
        public Task<int> GetTotalItemAsync();
    }
}
