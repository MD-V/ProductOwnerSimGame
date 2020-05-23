using ProductOwnerSimGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IOrganizationLogic
    {
        Task<string> CreateOrganizationAsync(string displayName);

        Task<bool> DeleteOrganizationAsync(string organizationId);

        Task<bool> EditOrganizationAsync(string organizationId, string displayName);

        Task<bool> AddUsersToOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds);

        Task<bool> RemoveUsersFromOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds);

        Task<Organization> GetOrganizationAsync(string organizationId);

        Task<IReadOnlyCollection<Organization>> GetOrganizationsAsync();
        
        Task<Organization> GetOrganizationByUserAsync(string userId);
    }
}
