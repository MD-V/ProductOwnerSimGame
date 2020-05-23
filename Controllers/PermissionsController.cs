using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionLogic _PermissionLogic;

        public PermissionsController(IPermissionLogic permissionLogic)
        {
            _PermissionLogic = permissionLogic;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions(string userNameOrEmail)
        {
            return Ok(await _PermissionLogic.GetGrantedPermissionsAsync(userNameOrEmail));
        }
    }
}