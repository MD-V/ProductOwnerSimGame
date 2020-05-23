using ProductOwnerSimGame.Dtos;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.Requests.User
{
    public class CreateOrUpdateUserRequest
    {
        public UserDto User { get; set; } = new UserDto();

        public string GrantedRoleId { get; set; }
    }
}
