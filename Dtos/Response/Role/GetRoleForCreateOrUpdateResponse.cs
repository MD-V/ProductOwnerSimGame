using ProductOwnerSimGame.Dtos;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.Response.Role
{
    public class GetRoleForCreateOrUpdateResponse
    {
        public RoleDto Role { get; set; } = new RoleDto();

        public List<PermissionDto> AllPermissions { get; set; } = new List<PermissionDto>();

        public List<Guid> GrantedPermissionIds { get; set; } = new List<Guid>();
    }
}