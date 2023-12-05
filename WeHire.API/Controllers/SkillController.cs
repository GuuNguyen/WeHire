using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.CompanyPartner;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.SkillServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;
        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetSkillDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSkillAsync([FromQuery] PagingQuery query,
                                                          [FromQuery] SearchSkillDTO searchKey)
        {
            var result = _skillService.GetAllSkill(query, searchKey);
            var total = await _skillService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageSize,
                Size = query.PageSize,
                Total = total,
            };
            return Ok(new PagedApiResponse<GetSkillDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result,
                Paging = paging
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetSkillDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateEmployeeAsync(CreateSkillDTO requestBody)
        {
            var result = await _skillService.CreateSkillAsync(requestBody);

            return Created(string.Empty, new ApiResponse<GetSkillDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result,
            });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSkillAsync(UpdateSkillModel requestBody)
        {
            await _skillService.UpdateSkillAsync(requestBody);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Update success",
            });
        }

        [HttpDelete("{skillId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteSkillAsync(int skillId)
        {
            await _skillService.DeleteSkillAsync(skillId);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Delete success",
            });
        }
    }
}
