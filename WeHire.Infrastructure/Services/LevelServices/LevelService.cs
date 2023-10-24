using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Level;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Entity.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.LevelEnum;

namespace WeHire.Infrastructure.Services.LevelServices
{
    public class LevelService : ILevelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LevelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetLevelDetail> GetAllLevel(PagingQuery query, SearchLevelDTO searchKey)
        {
            var levels = _unitOfWork.LevelRepository.GetAll();
            
            levels = levels.SearchItems(searchKey);

            levels = levels.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var mappedLevels = _mapper.Map<List<GetLevelDetail>>(levels);
            return mappedLevels;
        }

        public async Task<GetLevelDetail> CreateLevelAsync(CreateLevelDTO requestBody)
        {
            if(requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);
            var newLevel = _mapper.Map<Level>(requestBody);
            newLevel.Status = (int)LevelStatus.Active;

            await _unitOfWork.LevelRepository.InsertAsync(newLevel);
            await _unitOfWork.SaveChangesAsync();

            var mappedLevel = _mapper.Map<GetLevelDetail>(newLevel);
            return mappedLevel;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.LevelRepository.GetAll().CountAsync();
            return total;
        }
    }
}
