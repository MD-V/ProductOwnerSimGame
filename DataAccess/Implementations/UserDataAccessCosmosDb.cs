using Microsoft.Azure.Cosmos;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Implementations
{
    public class UserDataAccessCosmosDb : DataAccessCosmosDbBase, IUserDataAccess
    {
        private const string _DatabaseName = "db";
        private const int _DatabaseThroughput = 400;

        private const string _UserContainerName = "users";
        private const int _UserContainerThroughput = 400;
        private const string _UserContainerPartitionKeyPath = "/UserId";

        public UserDataAccessCosmosDb(string connectionString) : base(connectionString)
        {

        }

        public async Task<bool> CreateUserAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var createItemResponse = await container.CreateItemAsync(user, new PartitionKey(user.UserId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.user_id = @user_id")
                .WithParameter("@user_id", userId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User user = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            var deleteItemResponse = await container.DeleteItemAsync<Organization>(user.UserId, new PartitionKey(user.UserId));

            if (deleteItemResponse.StatusCode == HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
        }

        public async Task<Models.Users.User> FindByEmailAsync(string normalizedEmail)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE UPPER(c.NormalizedEmail) = @NormalizedEmail")
                .WithParameter("@NormalizedEmail", normalizedEmail);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User user = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            return user;
        }

        public async Task<Models.Users.User> FindByNameAsync(string normalizedUserName)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE UPPER(c.NormalizedUserName) = @NormalizedUserName")
                .WithParameter("@NormalizedUserName", normalizedUserName);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User user = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            return user;
        }

        public async Task<string> GetEmailAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return userDb.Email;
        }

        public async Task<bool> GetEmailConfirmedAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return userDb.EmailConfirmed;
        }

        public async Task<string> GetNormalizedEmailAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return userDb.NormalizedEmail;
        }

        public async Task<string> GetNormalizedUserNameAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return userDb.NormalizedUserName;
        }

        public async Task<string> GetPasswordHashAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return userDb.PasswordHash;
        }

        public async Task<Models.Users.User> GetUserAsync(string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", userId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User user = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            return user;
        }

        public Task<string> GetUserIdAsync(Models.Users.User user)
        {
            // TODO?
            return Task.FromResult(user.UserId);
        }

        public async Task<string> GetUserNameAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return user.UserName;
        }

        public IQueryable<Models.Users.User> GetUsersAsIQueryable()
        {
            var container = CosmosClient.GetContainer(_DatabaseName, _UserContainerName);

            

            return  container.GetItemLinqQueryable<Models.Users.User>(allowSynchronousQueryExecution: true).ToList().AsQueryable();
        }

        public async Task<IReadOnlyCollection<Models.Users.User>> GetUsersAsync()
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var organizations = container.GetItemLinqQueryable<Models.Users.User>();

            return organizations.ToList();
        }

        public async Task<bool> HasPasswordAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                userDb = response.FirstOrDefault();
            }

            return !string.IsNullOrEmpty(userDb.PasswordHash);
        }

        public async Task SetEmailAsync(Models.Users.User user, string email)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                userDb.Email = email;

                await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));
            }
        }

        public async Task SetEmailConfirmedAsync(Models.Users.User user, bool confirmed)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                userDb.EmailConfirmed = confirmed;

                await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));
            }
        }

        public async Task SetNormalizedEmailAsync(Models.Users.User user, string normalizedEmail)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                userDb.NormalizedEmail = normalizedEmail;

                await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));
            }
        }

        public async Task SetNormalizedUserNameAsync(Models.Users.User user, string normalizedName)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                user.NormalizedUserName = normalizedName;

                await container.UpsertItemAsync(user, new PartitionKey(user.UserId));
            }
        }

        public async Task SetPasswordHashAsync(Models.Users.User user, string passwordHash)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                userDb.PasswordHash = passwordHash;

                await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));
            }
        }

        public async Task SetUserNameAsync(Models.Users.User user, string userName)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return;
            }
            else
            {
                userDb.UserName = userName;

                await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));
            }
        }

        public async Task<bool> UpdateUserAsync(Models.Users.User user)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_UserContainerName, _UserContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _UserContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_UserContainerName} c WHERE c.UserId = @UserId")
                .WithParameter("@UserId", user.UserId);

            var queryIterator = container.GetItemQueryIterator<Models.Users.User>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Models.Users.User userDb = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                user = response.FirstOrDefault();
            }

            if (userDb == null)
            {
                return false;
            }
            else
            {
                userDb = user;

                var upsertItemResponse = await container.UpsertItemAsync(userDb, new PartitionKey(user.UserId));

                if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
