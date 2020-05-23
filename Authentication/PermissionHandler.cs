using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ProductOwnerSimGame.Logic;

namespace ProductOwnerSimGame.Authentication
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionLogic _PermissionLogic;

        public PermissionHandler(IPermissionLogic permissionLogic)
        {
            _PermissionLogic = permissionLogic;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var hasPermission = await _PermissionLogic.IsUserGrantedToPermissionAsync(context.User.Identity.Name, requirement.Permission.Name);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
