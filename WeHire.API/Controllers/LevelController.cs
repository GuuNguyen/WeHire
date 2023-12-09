using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Developer;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Skill;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.LevelServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly ILevelService _levelService;

        public LevelController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetLevelDetail>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllLevelAsync([FromQuery] PagingQuery query, 
                                                          [FromQuery] SearchLevelDTO searchKey)
        {
            var result = _levelService.GetAllLevel(query, searchKey);
            var total = await _levelService.GetTotalItemAsync();
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetLevelDetail>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = result
            });
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetLevelDetail>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateLevelAsync(CreateLevelDTO requestBody)
        {
            var result = await _levelService.CreateLevelAsync(requestBody);
            return Created(string.Empty, new ApiResponse<GetLevelDetail>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateLevelAsync(UpdateLevelModel requestBody)
        {
            await _levelService.UpdateLevelAsync(requestBody);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Update success",
            });
        }

        [HttpDelete("{levelId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteLevelAsync(int levelId)
        {
            await _levelService.DeleteLevelAsync(levelId);
            return Ok(new ApiResponse<string>()
            {
                Code = StatusCodes.Status200OK,
                Data = "Delete success",
            });
        }
    }
}
