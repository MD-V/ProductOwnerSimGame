using Microsoft.Azure.Cosmos;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Implementations
{
    public class OrganizationDataAccessCosmosDb : DataAccessCosmosDbBase, IOrganizationDataAccess
    {
        private const string _DatabaseName = "db";
        private const int _DatabaseThroughput = 400;

        private const string _OrganizationContainerName = "organizations";
        private const int _OrganizationContainerThroughput = 400;
        private const string _OrganizationContainerPartitionKeyPath = "/org_id";

        public OrganizationDataAccessCosmosDb(string connectionString) : base(connectionString)
        {

        }

        public async Task<string> CreateOrganizationAsync(string displayName)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var org  = new Organization
            {
                DisplayName = displayName,
            };

            var createItemResponse = await container.CreateItemAsync(org, new PartitionKey(org.OrganizationId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return org.OrganizationId;
            }

            return string.Empty;
        }

        public async Task<bool> DeleteOrganizationAsync(string organizationId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE c.org_id = @org_id")
                .WithParameter("@org_id", organizationId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            var deleteItemResponse = await container.DeleteItemAsync<Organization>(organization.Id, new PartitionKey(organization.OrganizationId));

            if (deleteItemResponse.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> EditOrganizationAsync(string organizationId, string displayName)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE c.org_id = @org_id")
                .WithParameter("@org_id", organizationId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            if (organization == null)
            {
                return false;
            }
            else
            {
                organization.DisplayName = displayName;

                var createItemResponse = await container.UpsertItemAsync(organization, new PartitionKey(organization.OrganizationId));

                if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<Organization> GetOrganizationAsync(string organizationId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE c.org_id = @org_id")
                .WithParameter("@org_id", organizationId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            return organization;
        }

        public async Task<IReadOnlyCollection<Organization>> GetOrganizationsAsync()
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var organizations = container.GetItemLinqQueryable<Organization>();

            return organizations.ToList();
        }

        public async Task<bool> AddUsersToOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE c.org_id = @org_id")
                .WithParameter("@org_id", organizationId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            if (organization == null)
            {
                return false;
            }
            else
            {
                organization.UserIds.AddRange(userIds);

                var createItemResponse = await container.UpsertItemAsync(organization, new PartitionKey(organization.OrganizationId));

                if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> RemoveUsersFromOrganizationAsync(string organizationId, IReadOnlyCollection<string> userIds)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE c.org_id = @org_id")
                .WithParameter("@org_id", organizationId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            if (organization == null)
            {
                return false;
            }
            else
            {
                foreach(var userId in userIds)
                {
                    organization.UserIds.Remove(userId);
                }

                var createItemResponse = await container.UpsertItemAsync(organization, new PartitionKey(organization.OrganizationId));

                if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<Organization> GetOrganizationByUserAsync(string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_OrganizationContainerName, _OrganizationContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _OrganizationContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_OrganizationContainerName} c WHERE ARRAY_CONTAINS(c.usr_ids, @userId)")
                .WithParameter("@userId", userId);

            var queryIterator = container.GetItemQueryIterator<Organization>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Organization organization = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                organization = response.FirstOrDefault();
            }

            return organization;
        }
    }
}
