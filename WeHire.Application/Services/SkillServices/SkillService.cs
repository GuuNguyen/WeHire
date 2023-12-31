﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Utilities.Helper.Searching;
using WeHire.Domain.Entities;
using WeHire.Domain.Enums;
using WeHire.Infrastructure.IRepositories;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;
using static WeHire.Domain.Enums.SkillEnum;

namespace WeHire.Application.Services.SkillServices
{
    public class SkillService : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SkillService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<GetSkillDetail> GetAllSkill(PagingQuery query, SearchSkillDTO searchKey)
        {
            var skills = _unitOfWork.SkillRepository.GetAll().AsNoTracking();

            skills = skills.SearchItems(searchKey); 

            skills = skills.PagedItems(query.PageIndex, query.PageSize).AsQueryable();

            var listSkillMapped = _mapper.Map<List<GetSkillDetail>>(skills);
            return listSkillMapped;
        }

        public async Task<GetSkillDetail> CreateSkillAsync(CreateSkillDTO requestBody)
        {
            if (requestBody == null)
                throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.REQUEST_BODY, ErrorMessage.NULL_REQUEST_BODY);
            
            var newSkill = _mapper.Map<Skill>(requestBody);
            newSkill.Status = (int)SkillStatus.Active;

            await _unitOfWork.SkillRepository.InsertAsync(newSkill);
            await _unitOfWork.SaveChangesAsync();

            var skillDetail = _mapper.Map<GetSkillDetail>(newSkill);
            return skillDetail;
        }

        public async Task<int> GetTotalItemAsync()
        {
            var total = await _unitOfWork.SkillRepository.GetAll().AsNoTracking().CountAsync();
            return total;
        }

        public async Task UpdateSkillAsync(UpdateSkillModel requestBody)
        {
            var skill = await _unitOfWork.SkillRepository.GetByIdAsync(requestBody.SkillId)
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SKILL_FIELD, ErrorMessage.SKILL_NOT_EXIST);
            var updatedSkill = _mapper.Map(requestBody, skill);
            _unitOfWork.SkillRepository.Update(updatedSkill);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSkillAsync(int skillId)
        {
            var skill = await _unitOfWork.SkillRepository.GetByIdAsync(skillId)
              ?? throw new ExceptionResponse(HttpStatusCode.BadRequest, ErrorField.SKILL_FIELD, ErrorMessage.SKILL_NOT_EXIST);
            skill.Status = (int)SkillStatus.Inactive;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
