﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Type;
using WeHire.Application.Utilities.Helper.Pagination;

namespace WeHire.Application.Services.TypeServices
{
    public interface ITypeService
    {
        public List<GetTypeDetail> GetAllType(PagingQuery query, SearchTypeDTO searchKey);
        public Task<GetTypeDetail> CreateTypeAsync(CreateTypeDTO requestBody);
        public Task UpdateTypeAsync(UpdateTypeModel requestBody);
        public Task DeleteTypeAsync(int typeId);
        public Task<int> GetTotalItemAsync();
    }
}
