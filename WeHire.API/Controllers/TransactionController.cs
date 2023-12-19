using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Application.Services.TransactionServices;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.Helper.CheckNullProperties;
using Microsoft.AspNetCore.Authorization;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetTransactionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTransaction([FromQuery] PagingQuery query,
                                                        [FromQuery] SearchTransactionDTO searchKey)
        {
            var result = _transactionService.GetTransactions(searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() ? await _transactionService.GetTotalItemAsync()
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetTransactionDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }

        [Authorize]
        [HttpGet("ByCompany")]
        [ProducesResponseType(typeof(PagedApiResponse<GetTransactionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactionByCompany(int companyId,
                                                        [FromQuery] PagingQuery query, 
                                                        [FromQuery] SearchTransactionDTO searchKey)
        {
            var result = await _transactionService.GetTransactionsByCompanyIdAsync(companyId, searchKey);
            var pagingResult = result.PagedItems(query.PageIndex, query.PageSize).ToList();

            var total = searchKey.AreAllPropertiesNull() ? await _transactionService.GetTotalItemAsync(companyId)
                                                         : result.Count;
            var paging = new PaginationInfo
            {
                Page = query.PageIndex,
                Size = query.PageSize,
                Total = total
            };
            return Ok(new PagedApiResponse<GetTransactionDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = paging,
                Data = pagingResult
            });
        }
    }
}
