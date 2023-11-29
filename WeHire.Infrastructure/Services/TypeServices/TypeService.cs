using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Type;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Enums;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.TypeEnum;

namespace WeHire.Infrastructure.Services.TypeServices
{
    public class TypeService : ITypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetTypeDetail> GetAllType(PagingQuery query, SearchTypeDTO searchKey)
        {
            var types = _unitOfWork.TypeRepository.GetAll().AsNoTracking();

            types = types.SearchItems(searchKey);

            types = types.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedTypes = _mapper.Map<List<GetTypeDetail>>(types);   
            return mappedTypes;
        }

        public async Task<GetTypeDetail> CreateTypeAsync(CreateTypeDTO requestBody)
        {
            if (requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);
            var newType = _mapper.Map<WeHire.Domain.Entities.Type>(requestBody);
            newType.Status = (int)TypeEnum.TypeStatus.Active;

            await _unitOfWork.TypeRepository.InsertAsync(newType);
            await _unitOfWork.SaveChangesAsync();

            var mappedType = _mapper.Map<GetTypeDetail>(newType);
            return mappedType;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.TypeRepository.GetAll().AsNoTracking().CountAsync();
            return total;
        }

        public async Task UpdateTypeAsync(UpdateTypeModel requestBody)
        {
            var type = await _unitOfWork.TypeRepository.GetByIdAsync(requestBody.TypeId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TYPE_FIELD, ErrorMessage.TYPE_NOT_EXIST);
            var updatedType = _mapper.Map(requestBody, type);
            _unitOfWork.TypeRepository.Update(updatedType);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTypeAsync(int typeId)
        {
            var type = await _unitOfWork.TypeRepository.GetByIdAsync(typeId)
               ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.TYPE_FIELD, ErrorMessage.TYPE_NOT_EXIST);
            type.Status = (int)TypeStatus.Inactive;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
