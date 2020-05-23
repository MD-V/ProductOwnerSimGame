using Microsoft.Azure.Cosmos;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models.GameVariant;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Implementations
{
    public class GameVariantDataAccessCosmosDb : DataAccessCosmosDbBase, IGameVariantDataAccess
    {
        private const string _DatabaseName = "db";
        private const int _DatabaseThroughput = 400;

        private const string _GameVariantContainerName = "gamevariants";
        private const int _GameVariantContainerThroughput = 400;
        private const string _GameVariantContainerPartitionKeyPath = "/gv_id";

        public GameVariantDataAccessCosmosDb(string connectionString) : base(connectionString)
        {

        }

        public async Task<string> CreateGameVariantAsync(GameVariant gameVariant)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GameVariantContainerName, _GameVariantContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GameVariantContainerThroughput);

            var container = containerResponse.Container;

            var createItemResponse = await container.CreateItemAsync(gameVariant, new PartitionKey(gameVariant.GameVariantId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return gameVariant.GameVariantId;
            }

            return null;
        }

        public async Task<GameVariant> GetGameVariantAsync(string gameVariantId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GameVariantContainerName, _GameVariantContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GameVariantContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GameVariantContainerName} c WHERE c.gv_id = @gv_id")
                .WithParameter("@gv_id", gameVariantId);

            var queryIterator = container.GetItemQueryIterator<GameVariant>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

           GameVariant gameVariant = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                gameVariant = response.FirstOrDefault();
            }

            return gameVariant;
        }

        public async Task<IReadOnlyCollection<GameVariant>> GetGameVariantsAsync(string organizationId, int playerCount = 0)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GameVariantContainerName, _GameVariantContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GameVariantContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GameVariantContainerName} c WHERE (c.org_id = @org_id OR IS_NULL(c.org_id)) AND c.pl_count = @pl_count")
                .WithParameter("@org_id", organizationId).WithParameter("@pl_count", playerCount);

            var queryIterator = container.GetItemQueryIterator<GameVariant>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            List<GameVariant> gameVariants = new List<GameVariant>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                gameVariants.AddRange(response);
            }

            return gameVariants;
        }
    }
}
