using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IPermissionLogic
    {
        Task<IEnumerable<PermissionDto>> GetGrantedPermissionsAsync(string userNameOrEmail);

        Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName);

        void InitializePermissions(List<Permission> permissions);
    }
}
