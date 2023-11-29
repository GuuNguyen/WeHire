using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;
using WeHire.Application.DTOs.Contract;
using WeHire.Application.Utilities.Helper.Pagination;
using WeHire.Infrastructure.Services.TransactionServices;
using WeHire.Application.DTOs.Transaction;
using WeHire.Application.Utilities.Helper.CheckNullProperties;

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


        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<GetTransactionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPreContract([FromQuery] PagingQuery query,
                                                        [FromQuery] SearchTransactionDTO searchKey)
        {
            var result = _transactionService.GetTransactions(query, searchKey);
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
                Data = result
            });
        }

        [HttpGet("ByCompany")]
        [ProducesResponseType(typeof(PagedApiResponse<GetTransactionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPreContract(int companyId,
                                                        [FromQuery] PagingQuery query, 
                                                        [FromQuery] SearchTransactionDTO searchKey)
        {
            var result = await _transactionService.GetTransactionsByCompanyIdAsync(companyId, query, searchKey);
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
                Data = result
            });
        }
    }
}
