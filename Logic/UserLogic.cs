using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductOwnerSimGame.Authentication;
using ProductOwnerSimGame.Dtos.Requests.User;
using ProductOwnerSimGame.Dtos.Response.User;
using ProductOwnerSimGame.Models.Roles;
using ProductOwnerSimGame.Models.Users;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly UserManager<User> _UserManager;

        private readonly IMapper _Mapper;

        private readonly JwtTokenConfiguration _jwtTokenConfiguration;
        private readonly IConfiguration _configuration;
        readonly ILogger<UserLogic> _logger;

        public UserLogic(
            UserManager<User> userManager, 
            IMapper mapper, 
            IOptions<JwtTokenConfiguration> jwtTokenConfiguration,
            IConfiguration configuration, 
            ILogger<UserLogic> logger)
        {
            _UserManager = userManager;
            _Mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _jwtTokenConfiguration = jwtTokenConfiguration.Value;
        }

        public async Task<IdentityResult> AddUserAsync(CreateOrUpdateUserRequest input)
        {
            var user = new User
            {
                Id = input.User.Id,
                UserName = input.User.UserName,
                Email = input.User.Email
            };

            return await _UserManager.CreateAsync(user, input.User.Password);
        }

        public async Task<SecurityToken> CreateTokenAsync(string userNameOrEmail, string password)
        {
            var userToVerify = await CreateClaimsIdentityAsync(userNameOrEmail, password);
            if (userToVerify == null)
            {
                return null;
            }

            var token = new JwtSecurityToken
            (
                issuer: _jwtTokenConfiguration.Issuer,
                audience: _jwtTokenConfiguration.Audience,
                claims: userToVerify.Claims,
                expires: _jwtTokenConfiguration.EndDate,
                notBefore: _jwtTokenConfiguration.StartDate,
                signingCredentials: _jwtTokenConfiguration.SigningCredentials
            );

            return token;
        }

        private async Task<ClaimsIdentity> CreateClaimsIdentityAsync(string userNameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(userNameOrEmail) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var userToVerify = await FindUserByUserNameOrEmail(userNameOrEmail);

            if (userToVerify == null)
            {
                return null;
            }

            if (await _UserManager.CheckPasswordAsync(userToVerify, password))
            {
                return new ClaimsIdentity(new GenericIdentity(userNameOrEmail, "Token"), new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userNameOrEmail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userToVerify.Id.ToString())
                });
            }

            return null;
        }


        public async Task<IdentityResult> EditUserAsync(CreateOrUpdateUserRequest input)
        {
            var user = await _UserManager.FindByIdAsync(input.User.Id.ToString());
            if (user.UserName == input.User.UserName && user.Id != input.User.Id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNameAlreadyExist",
                    Description = "This user name is already exist!"
                });
            }

            if (!string.IsNullOrEmpty(input.User.Password))
            {
                var changePasswordResult = await ChangePassword(user, input.User.Password);
                if (!changePasswordResult.Succeeded)
                {
                    return changePasswordResult;
                }
            }

            return await UpdateUser(input, user);
        }

        

        public async Task<IEnumerable<UserListResponse>> GetUsersAsync(UserListRequest input)
        {
            var query = _UserManager.Users.Select(a => new UserListResponse()
            {
                Email = a.Email,
                UserName = a.UserName
            });
            return query;
        }

        public async Task<IdentityResult> RemoveUserAsync(Guid id)
        {
            var user = _UserManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found!"
                });
            }

            if (IntegratedUsers.All().Select(u => u.UserName).Contains(user.UserName))
            {
                return IdentityResult.Failed(new IdentityError()
                {
                    Code = "CannotRemoveSystemUser",
                    Description = "You cannot remove system user!"
                });
            }

            var removeUserResult = await _UserManager.DeleteAsync(user);
            if (!removeUserResult.Succeeded)
            {
                return removeUserResult;
            }


            return removeUserResult;
        }

       

        private async Task<IdentityResult> ChangePassword(User user, string password)
        {
            var changePasswordResult = await _UserManager.RemovePasswordAsync(user);
            if (changePasswordResult.Succeeded)
            {
                changePasswordResult = await _UserManager.AddPasswordAsync(user, password);
            }

            return changePasswordResult;
        }

        private async Task<IdentityResult> UpdateUser(CreateOrUpdateUserRequest input, User user)
        {
            user.UserName = input.User.UserName;
            user.Email = input.User.Email;
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.UserRoleId = input.GrantedRoleId;
            return await _UserManager.UpdateAsync(user);
            
        }

        public async Task<User> GetUserAsync(string userNameOrEmail)
        {
            return await FindUserByUserNameOrEmail(userNameOrEmail).ConfigureAwait(false);
        }

        private async Task<User> FindUserByUserNameOrEmail(string userNameOrEmail)
        {
            return await _UserManager.FindByNameAsync(userNameOrEmail).ConfigureAwait(false) ??
                   await _UserManager.FindByEmailAsync(userNameOrEmail).ConfigureAwait(false);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _UserManager.FindByIdAsync(userId).ConfigureAwait(false);
        }
    }
}
