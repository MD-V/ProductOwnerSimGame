using Microsoft.AspNetCore.Authorization;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Authentication
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; }
    }
}
