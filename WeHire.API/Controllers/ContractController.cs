using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.ContractServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetListContract>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllContract([FromQuery] PagingQuery query, [FromQuery] SearchContractDTO searchKey)
        {
            var result = _contractService.GetContractAsync(searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() ? await _contractService.GetTotalItemAsync()
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetListContract>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("ByCompany")]
        [ProducesResponseType(typeof(PagedApiResponse<GetListContract>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllContractByCompany(int companyId, [FromQuery] PagingQuery query, [FromQuery] SearchContractDTO searchKey)
        {
            var result = _contractService.GetContractByCompanyAsync(companyId, searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() ? await _contractService.GetTotalItemAsync(companyId)
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetListContract>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("{contractId}")]
        [ProducesResponseType(typeof(ApiResponse<GetContractDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContractById(int contractId)
        {
            var result = await _contractService.GetContractByIdAsync(contractId);
            return Ok(new ApiResponse<GetContractDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("PreContract")]
        [ProducesResponseType(typeof(ApiResponse<GetPreContract>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPreContract(int developerId, int requestId)
        {
            var result = await _contractService.GetPreContractAsync(developerId, requestId);
            return Ok(new ApiResponse<GetPreContract>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpGet("ContractByDev")]
        [ProducesResponseType(typeof(ApiResponse<GetContractDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContractByDev(int developerId, int projectId)
        {
            var result = await _contractService.GetContractByDevAsync(developerId, projectId);
            return Ok(new ApiResponse<GetContractDetail>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GetContractDTO>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateContract(CreateContractDTO requestBody)
        {
            var result = await _contractService.CreateContractAsync(requestBody);
            return Created(string.Empty, new ApiResponse<GetContractDTO>()
            {
                Code = StatusCodes.Status201Created,
                Data = result
            });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPut("ConfirmSigned")]
        [ProducesResponseType(typeof(ApiResponse<GetContractDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ConfirmSignedContract(int contractId)
        {
            var result = await _contractService.ConfirmSignedContractAsync(contractId);
            return Created(string.Empty, new ApiResponse<GetContractDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPut("FailContract")]
        [ProducesResponseType(typeof(ApiResponse<GetContractDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FailContract(int projectId)
        {
            var result = await _contractService.FailContractAsync(projectId);
            return Created(string.Empty, new ApiResponse<GetContractDTO>()
            {
                Code = StatusCodes.Status200OK,
                Data = result
            });
        }
    }
}
