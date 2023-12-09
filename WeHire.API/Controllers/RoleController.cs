using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeHire.Application.DTOs.Level;
using WeHire.Application.DTOs.Role;
using WeHire.Application.Services.RoleServices;
using static WeHire.Application.Utilities.ResponseHandler.ResponseModel;

namespace WeHire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<List<GetRoleDTO>>), StatusCodes.Status200OK)]
        public IActionResult GetAllRole()
        {
            var result = _roleService.GetAllRole();
            return Ok(new PagedApiResponse<GetRoleDTO>()
            {
                Code = StatusCodes.Status200OK,
                Paging = null,
                Data = result
            });
        }
    }
}
