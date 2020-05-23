using Microsoft.Azure.Cosmos;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.GameVariant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Implementations
{
    public class GameDataAccessCosmosDb : DataAccessCosmosDbBase, IGameDataAccess
    {
        private const string _DatabaseName = "db";
        private const int _DatabaseThroughput = 400;

        private const string _GamesContainerName = "games";
        private const int _GamesContainerThroughput = 400;
        private const string _GamesContainerPartitionKeyPath = "/game_id";

        public GameDataAccessCosmosDb(string connectionString) : base(connectionString)
        {

        }

        public async Task<Game> GetGameAsync(string gameId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            return game;
        }


        public async Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.gm_id = @user_id OR ARRAY_CONTAINS(c.player_ids, @user_id)")
                .WithParameter("@user_id", userId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            List<Game> gamesList = new List<Game>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                gamesList.AddRange(response);
            }

            return gamesList;
        }



        public async Task<Game> CreateNewGameAsync(GameVariant gameVariant, string organizationId, string gameMasterUserId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var game = new Game
            {             
                GameMasterId = gameMasterUserId,
                OrganizationId = organizationId,
                GameVariantId = gameVariant.GameVariantId,
                InitialValues = gameVariant.InitialProjectValues,
                AccessCode = CreateAccessCode(),
                State = GameStatusEnum.WaitingForPlayers
            };

            var createItemResponse = await container.CreateItemAsync(game, new PartitionKey(game.GameId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return game;
            }

            return null;
        }

        public async Task<bool> StartGameAsync(string gameId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if(game == null)
            {
                return false;
            }

            game.State = GameStatusEnum.Started;

            var createItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;

        }

        private string CreateAccessCode(int length = 5)
        {
            
            // creating a StringBuilder object()
            var str_build = new StringBuilder();
            var random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                var flt = random.NextDouble();
                var shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }

        public async Task<bool> RemovePlayerFromGameAsync(string gameId, string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.PlayerIds.Remove(userId);

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AddPlayerToGameAsync(string gameId, string userId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.PlayerIds.Add(userId);

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AssignRoleToPlayerAsync(string gameId, string userId, GameRole gameRole)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.PlayerRoles[userId] = gameRole;

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateGameStatusAsync(string gameId, GameStatusEnum gameStatus)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.State = gameStatus;

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateGamePhaseAsync(string gameId, string gamePhaseId, int sequence)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.CurrentGamePhaseId = gamePhaseId;
            game.CurrentGamePhaseSequence = sequence;

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SaveDecisionAsync(string gameId, Decision decision)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.Decisions.Add(decision);

            var upsertItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (upsertItemResponse.StatusCode == HttpStatusCode.Created || upsertItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public async Task<Game> GetGameByAccessCodeAsync(string accessCode)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.acc_code = @acc_code")
                .WithParameter("@acc_code", accessCode);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            return game;
        }

        public async Task<IReadOnlyCollection<string>> GetRunningGameIdsAsync()
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.state = 'Started' or c.state = 'InitialMissionScreen'");

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            List<string> gamesList = new List<string>();

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                gamesList.AddRange(response.Select(a=> a.GameId));
            }

            return gamesList;
        }

        public async Task<bool> ClearRolesAsync(string gameId)
        {
            var createResponse = await CosmosClient.CreateDatabaseIfNotExistsAsync(_DatabaseName, _DatabaseThroughput);

            var database = createResponse.Database;

            var containerProperties = new ContainerProperties(_GamesContainerName, _GamesContainerPartitionKeyPath);

            var containerResponse = await database.CreateContainerIfNotExistsAsync(
                containerProperties,
                _GamesContainerThroughput);

            var container = containerResponse.Container;

            var query = new QueryDefinition($"SELECT * FROM {_GamesContainerName} c WHERE c.game_id = @game_id")
                .WithParameter("@game_id", gameId);

            var queryIterator = container.GetItemQueryIterator<Game>(query, requestOptions: new QueryRequestOptions() { MaxConcurrency = 1 });

            Game game = null;

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync();
                game = response.FirstOrDefault();
            }

            if (game == null)
            {
                return false;
            }

            game.PlayerRoles.Clear();

            var createItemResponse = await container.UpsertItemAsync(game, new PartitionKey(game.GameId));

            if (createItemResponse.StatusCode == HttpStatusCode.Created || createItemResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
    }
}
