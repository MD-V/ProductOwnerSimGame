using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Dtos.Requests.User;
using ProductOwnerSimGame.Dtos.Response.User;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;
using ProductOwnerSimGame.Models.Roles;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserLogic _UserLogic;

        public UserController(IUserLogic userLogic)
        {
            _UserLogic = userLogic;     
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]Loginrequest input)
        {
            var token = await _UserLogic.CreateTokenAsync(input.UserNameOrEmail, input.Password).ConfigureAwait(false);


            return Ok(new LoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]RegisterRequest input)
        {
            throw new NotImplementedException();

            /*
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user != null)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new NameValueDto("EmailAlreadyExist", "This email already exists!")
                });
            }

            var applicationUser = new User
            {
                UserName = input.UserName,
                Email = input.Email,
                EmailConfirmed = true,
                UserRoleId = IntegratedRoles.User.RoleId
            };

            var result = await _userManager.CreateAsync(applicationUser, input.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
            */
        }

        [HttpPost("changepassword")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest input)
        {
            throw new NotImplementedException();
            /*
            if (input.NewPassword != input.PasswordRepeat)
            {
                return BadRequest(new List<NameValueDto>
                {
                    new NameValueDto("PasswordsDoesNotMatch", "Passwords doesn't match!")
                });
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
            */
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordRequest input)
        {
            throw new NotImplementedException();
            
            /*
            var user = await _userManager.FindUserByUserNameOrEmail(input.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new List<NameValueDto>
                {
                    new NameValueDto("UserNotFound", "User is not found!")
                });
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = _configuration["App:ClientUrl"] + "/account/reset-password?token=" + resetToken;
            var message = new MailMessage(
                from: _configuration["Email:Smtp:Username"],
                to: "alirizaadiyahsi@gmail.com",
                subject: "Reset your password",
                body: $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>"
            );
            message.IsBodyHtml = true;

            _logger.LogInformation(Environment.NewLine + Environment.NewLine +
                                   "******************* Reset Password Link *******************" +
                                   Environment.NewLine + Environment.NewLine +
                                   callbackUrl +
                                   Environment.NewLine + Environment.NewLine +
                                   "***********************************************************" +
                                   Environment.NewLine);
            return Ok(new ForgotPasswordResponse { ResetToken = resetToken });
            */
        }

        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordrequest input)
        {
            throw new NotImplementedException();
            /*
            var user = await _userManager.FindUserByUserNameOrEmail(input.UserNameOrEmail);
            if (user == null)
            {
                return NotFound(new List<NameValueDto>
                {
                    new NameValueDto("UserNotFound", "User is not found!")
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new NameValueDto(e.Code, e.Description)).ToList());
            }

            return Ok();
            */
        }

        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        [HttpGet("info")]
        public async Task<ActionResult<UserResponse>> GetUserInfo()
        {
            var user = await _UserLogic.GetUserAsync(HttpContext.User.Identity.Name).ConfigureAwait(false);
            
            var userRole = IntegratedRoles.All().FirstOrDefault(a => a.RoleId.Equals(user.UserRoleId));

            var userRoleName = userRole.NormalizedName.ToLower();

            var userResponse = new UserResponse()
            {
                EMail = user.Email,
                UserId = user.UserId,
                Username = user.UserName,
                Roles = new[] { userRoleName }
            };

            return Ok(userResponse);
        }

        /*
        [HttpGet("{id}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<ActionResult<GetUserForCreateOrUpdateResponse>> GetUser(Guid id)
        {
            //TODO
            return Ok();
        }
        */

        /*
        [HttpGet]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult<IEnumerable<UserListResponse>>> GetUsers([FromQuery]UserListRequest input)
        {
            return Ok(await _UserLogic.GetUsersAsync(input));
        }*/

        /*
        [HttpPost]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> PostUsers([FromBody]CreateOrUpdateUserRequest input)
        {
            var identityResult = await _UserLogic.AddUserAsync(input);

            if (identityResult.Succeeded)
            {
                return Created(Url.Action("PostUsers"), identityResult);
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpPut]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> PutUsers([FromBody]CreateOrUpdateUserRequest input)
        {
            var identityResult = await _UserLogic.EditUserAsync(input);

            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }

        [HttpDelete]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<ActionResult> DeleteUsers(Guid id)
        {
            var identityResult = await _UserLogic.RemoveUserAsync(id);

            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors.Select(e => new NameValueDto(e.Code, e.Description)));
        }
        */
    }
}
