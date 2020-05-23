using ProductOwnerSimGame.Models.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Interfaces
{
    public interface IUserDataAccess
    {     
        Task<bool> DeleteUserAsync(string userId);

        Task<User> GetUserAsync(string userId);
        
        Task<bool> CreateUserAsync(User user);
        
        Task<User> FindByNameAsync(string normalizedUserName);
        
        Task<string> GetNormalizedUserNameAsync(User user);
        
        
        IQueryable<User> GetUsersAsIQueryable();
        
        
        Task<string> GetUserIdAsync(User user);
       
        Task<string> GetUserNameAsync(User user);
        
        Task SetNormalizedUserNameAsync(User user, string normalizedName);
        
        Task SetUserNameAsync(User user, string userName);
        
        Task<bool> UpdateUserAsync(User user);
        
        Task<User> FindByEmailAsync(string normalizedEmail);
        
        Task<string> GetEmailAsync(User user);
        
        Task<bool> GetEmailConfirmedAsync(User user);
        
        Task<string> GetNormalizedEmailAsync(User user);
        
        Task SetEmailConfirmedAsync(User user, bool confirmed);
        
        Task SetEmailAsync(User user, string email);
        
        Task SetNormalizedEmailAsync(User user, string normalizedEmail);
        Task<string> GetPasswordHashAsync(User user);
        Task<bool> HasPasswordAsync(User user);
        Task SetPasswordHashAsync(User user, string passwordHash);
    }
}
