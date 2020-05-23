using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public class OrganizationLogic : IOrganizationLogic
    {
        private IOrganizationDataAccess _OrganizationDataAccess;

        public OrganizationLogic(IOrganizationDataAccess organizationDataAccess)
        {
            _OrganizationDataAccess = organizationDataAccess;
        }

        public async Task<string> CreateOrganizationAsync(string displayName)
        {
            return await _OrganizationDataAccess.CreateOrganizationAsync(displayName).ConfigureAwait(false);
        }

        public async Task<bool> DeleteOrganizationAsync(string organizationId)
        {
            return await _OrganizationDataAccess.DeleteOrganizationAsync(organizationId).ConfigureAwait(false);
        }

        public async Task<bool> EditOrganizationAsync(string organizationId, string displayName)
        {
            return await _OrganizationDataAccess.EditOrganizationAsync(organizationId, displayName).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<Organization>> GetOrganizationsAsync()
        {
            return await _OrganizationDataAccess.GetOrganizationsAsync().ConfigureAwait(false);
        }

        public async Task<Organization> GetOrganizationAsync(string organizationId)
        {
            return await _OrganizationDataAccess.GetOrganizationAsync(organizationId).ConfigureAwait(false);
        }

        public async Task<bool> RemoveUsersFromOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds)
        {
            return await _OrganizationDataAccess.RemoveUsersFromOrganizationAsync(organizationId, userIds).ConfigureAwait(false);
        }

        public async Task<bool> AddUsersToOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds)
        {
            return await _OrganizationDataAccess.AddUsersToOrganizationAsync(organizationId, userIds).ConfigureAwait(false);
        }

        public async Task<Organization> GetOrganizationByUserAsync(string userId)
        {
            return await _OrganizationDataAccess.GetOrganizationByUserAsync(userId).ConfigureAwait(false);
        }
    }
}
