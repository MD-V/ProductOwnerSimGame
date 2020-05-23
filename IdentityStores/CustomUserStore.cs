using Microsoft.AspNetCore.Identity;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models.Users;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.IdentityStores
{
    public class CustomUserStore : IUserStore<User>, IUserEmailStore<User>, IUserPasswordStore<User>, IQueryableUserStore<User>
    {
        private IUserDataAccess _UserDataAccess;

        public IQueryable<User> Users => _UserDataAccess.GetUsersAsIQueryable();

        public CustomUserStore(IUserDataAccess userDataAccess)
        {
            _UserDataAccess = userDataAccess;
        }
        
        
        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var createResult = await _UserDataAccess.CreateUserAsync(user).ConfigureAwait(false);

            if(createResult)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var createResult = await _UserDataAccess.DeleteUserAsync(user.UserId).ConfigureAwait(false);

            if (createResult)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        
        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.FindByEmailAsync(normalizedEmail).ConfigureAwait(false);
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetUserAsync(userId).ConfigureAwait(false);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.FindByNameAsync(normalizedUserName).ConfigureAwait(false);
        }

        public async Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetEmailAsync(user).ConfigureAwait(false);
        }

        public async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetEmailConfirmedAsync(user).ConfigureAwait(false);
        }

        public async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetNormalizedEmailAsync(user).ConfigureAwait(false);
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetNormalizedUserNameAsync(user).ConfigureAwait(false);
        }

        public async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetUserIdAsync(user).ConfigureAwait(false);
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetUserNameAsync(user).ConfigureAwait(false);
        }

        public async Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetEmailAsync(user, email).ConfigureAwait(false);
        }

        public async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetEmailConfirmedAsync(user, confirmed).ConfigureAwait(false);
        }

        public async Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetNormalizedEmailAsync(user, normalizedEmail).ConfigureAwait(false);
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetNormalizedUserNameAsync(user, normalizedName).ConfigureAwait(false);
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetUserNameAsync(user, userName).ConfigureAwait(false);
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var updateResult = await _UserDataAccess.UpdateUserAsync(user).ConfigureAwait(false);

            if (updateResult)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }

        public void Dispose()
        {
            // Do nothing
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.GetPasswordHashAsync(user).ConfigureAwait(false);
        }

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return await _UserDataAccess.HasPasswordAsync(user).ConfigureAwait(false);
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            await _UserDataAccess.SetPasswordHashAsync(user, passwordHash).ConfigureAwait(false);
        }
    }
}
