using System;
using System.Collections.Generic;
using ProductOwnerSimGame.Dtos;

namespace ProductOwnerSimGame.Dtos.Response.User
{
    public class GetUserForCreateOrUpdateResponse
    {
        public UserDto User { get; set; } = new UserDto();

        public List<RoleDto> AllRoles { get; set; } = new List<RoleDto>();

        public List<Guid> GrantedRoleIds { get; set; } = new List<Guid>();
    }
}