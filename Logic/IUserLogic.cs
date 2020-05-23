using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProductOwnerSimGame.Dtos.Requests.User;
using ProductOwnerSimGame.Dtos.Response.User;
using ProductOwnerSimGame.Models.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IUserLogic
    {
        Task<IEnumerable<UserListResponse>> GetUsersAsync(UserListRequest input);

        Task<IdentityResult> AddUserAsync(CreateOrUpdateUserRequest input);

        Task<IdentityResult> EditUserAsync(CreateOrUpdateUserRequest input);

        Task<IdentityResult> RemoveUserAsync(Guid id);


        Task<SecurityToken> CreateTokenAsync(string userNameOrEmail, string password);
        
        Task<User> GetUserAsync(string userNameOrEmail);

        Task<User> GetUserByIdAsync(string userId);
    }
}
