using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Dtos.Requests.Organization;
using ProductOwnerSimGame.Dtos.Response.Organization;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private IOrganizationLogic _OrganizationLogic;

        public OrganizationController(IOrganizationLogic organizationLogic)
        {
            _OrganizationLogic = organizationLogic;
        }

        [HttpGet]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<IActionResult> GetAllOrganizations()
        {
            return Ok(await _OrganizationLogic.GetOrganizationsAsync().ConfigureAwait(false));
        }

        [HttpGet("{organizationId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForOrgAdministration)]
        public async Task<IActionResult> GetOrganization(string organizationId)
        {

            return Ok(await _OrganizationLogic.GetOrganizationAsync(organizationId).ConfigureAwait(false));
        }

        [HttpPost]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest createOrganizationRequest)
        {
            return Ok(new CreateOrganizationResponse(await _OrganizationLogic.CreateOrganizationAsync(createOrganizationRequest?.DisplayName).ConfigureAwait(false)));
        }

       
        [HttpPut("{organizationId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<IActionResult> EditOrganization(string organizationId, [FromBody] string displayName)
        {
            return Ok(await _OrganizationLogic.EditOrganizationAsync(organizationId, displayName).ConfigureAwait(false));
        }

        
        [HttpDelete("{organizationId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<IActionResult> DeleteOrganzation(string organizationId)
        {
            return Ok(await _OrganizationLogic.DeleteOrganizationAsync(organizationId).ConfigureAwait(false));
        }

        [HttpPost("{organizationId}/adduser/{userId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForOrgAdministration)]
        public async Task<IActionResult> AddUserToOrganization(string organizationId, string userId)
        {
            return Ok(await _OrganizationLogic.AddUsersToOrganizationAsync(organizationId, new []{ userId}).ConfigureAwait(false));
        }

        [HttpDelete("{organizationId}/removeuser/{userId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForOrgAdministration)]
        public async Task<IActionResult> RemoveUserFromOrganization(string organizationId, string userId)
        {
            return Ok(await _OrganizationLogic.RemoveUsersFromOrganizationAsync(organizationId, new[] { userId }).ConfigureAwait(false));
        }
    }
}
