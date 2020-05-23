using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Hubs;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Simulation
{
    public class GameManager : IGameManager
    {
        private SemaphoreSlim _RunningGameSemaphore = new SemaphoreSlim(1, 1);

        private Dictionary<string, IGameProcessor> _RunningGames = new Dictionary<string, IGameProcessor>();

        private readonly IHubContext<GameViewHub, IGameViewHub> _GameViewHub;
        private readonly IGameDataAccess _GameDataAccess;
        private readonly IGameVariantDataAccess _GameVariantDataAccess;
        private readonly IOrganizationLogic _OrganizationLogic;
        private readonly IHttpContextAccessor _ContextAccessor;
        private readonly IServiceProvider _ServiceProvider;
        private readonly IMapper _Mapper;

        public GameManager(
            IHubContext<GameViewHub,
            IGameViewHub> gameViewHub,
            IServiceProvider serviceProvider,
            IGameDataAccess gameDataAccess,
            IGameVariantDataAccess gameVariantDataAccess,
            IOrganizationLogic organizationLogic,
            IHttpContextAccessor contextAccessor,
            IMapper mapper)
        {
            _GameViewHub = gameViewHub;
            _GameDataAccess = gameDataAccess;
            _GameVariantDataAccess = gameVariantDataAccess;
            _OrganizationLogic = organizationLogic;
            _ContextAccessor = contextAccessor;
            _ServiceProvider = serviceProvider;
            _Mapper = mapper;
        }


        public async Task Start()
        {
            //Load running games
            await StartPendingGames().ConfigureAwait(false);
        }

        public async Task<Game> CreateNewGameAsync(string gameVariantId, string organizationId, string gameMasterUserId)
        {
            var gameVariant = await _GameVariantDataAccess.GetGameVariantAsync(gameVariantId).ConfigureAwait(false);

            if(gameVariant == null)
            {
                return null;
            }

            return await _GameDataAccess.CreateNewGameAsync(gameVariant, organizationId, gameMasterUserId).ConfigureAwait(false);
        }

        private async Task StartPendingGames()
        {
            var gamesToStart = await _GameDataAccess.GetRunningGameIdsAsync().ConfigureAwait(false);

            foreach (var gameToStart in gamesToStart)
            {
                try
                {
                    await StartGameAsync(gameToStart).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    //TODO Log
                }
            }
        }

        public async Task<bool> StartGameAsync(string gameId)
        {

            if (await GameAlreadyExistsAsync(gameId).ConfigureAwait(false))
            {
                return false;
            }


            //Get game
            var game = await _GameDataAccess.GetGameAsync(gameId).ConfigureAwait(false);

            if (game == null)
            {
                return false;
            }

            if (game.State == GameStatusEnum.Finished || game.State == GameStatusEnum.Cancelled)
            {
                // Game already finished or cancelled
                return false;
            }

            //Get game variant
            var gameVariant = await _GameVariantDataAccess.GetGameVariantAsync(game.GameVariantId).ConfigureAwait(false);

            if (gameVariant == null)
            {
                return false;
            }


            if (game.PlayerIds.Count != gameVariant.PlayerCount)
            {
                // Not all players available
                return false;
            }

            if (game.State == GameStatusEnum.WaitingForPlayers)
            {
                if (game.PlayerRoles.Count != game.PlayerIds.Count)
                {
                    // clear roles in DB
                    var clearRoles = await _GameDataAccess.ClearRolesAsync(gameId).ConfigureAwait(false);

                    if (!clearRoles)
                    {
                        // Roles couldn't be cleared
                        return false;
                    }

                    // Assign random roles
                    var random = new Random();
                    int index = random.Next(game.PlayerIds.Count);

                    var productOwner = game.PlayerIds[index];
                    game.PlayerRoles.Add(productOwner, GameRole.ProductOwner);
                    var addProductOwner = await _GameDataAccess.AssignRoleToPlayerAsync(gameId, productOwner, GameRole.ProductOwner).ConfigureAwait(false);

                    if (!addProductOwner)
                    {
                        // Product owner wasnt added to db
                        return false;
                    }

                    var clonedPlayers = game.PlayerIds.ToList();
                    clonedPlayers.Remove(productOwner);

                    foreach (var stakeholder in clonedPlayers)
                    {
                        game.PlayerRoles.Add(stakeholder, GameRole.Stakeholder);

                        var addStakeholder = await _GameDataAccess.AssignRoleToPlayerAsync(gameId, stakeholder, GameRole.Stakeholder).ConfigureAwait(false);

                        if (!addStakeholder)
                        {
                            // Stakeholder wasnt added to db
                            return false;
                        }
                    }
                }
            }

            // Get players
            var playerDictionary = new Dictionary<string, GameRole>();

            using (var scope = _ServiceProvider.CreateScope())
            {
                var userLogic = scope.ServiceProvider.GetService(typeof(IUserLogic)) as IUserLogic;

                foreach (var player in game.PlayerRoles)
                {
                    var userId = player.Key;
                    var role = player.Value;

                    var user = await userLogic.GetUserByIdAsync(userId).ConfigureAwait(false);

                    playerDictionary.Add(user.UserId, role);
                }
            }

            if (playerDictionary.Count != game.PlayerRoles.Count)
            {
                // Not all players exist
                return false;
            }

            if (playerDictionary.Values.Where(a => a == GameRole.ProductOwner).Count() > 1)
            {
                // More than one product Owner
                return false;
            }


            // Add GameProcessor for game
            IGameProcessor gameProcessor = new GameProcessor(game, gameVariant, playerDictionary, _Mapper);
            // Subscribe to GameProcessor events
            gameProcessor.GameFinishedAsyncCallback = OnGameFinishedAsync;
            gameProcessor.SaveGameStatusAsyncCallback = OnSaveGameStatusAsyncCallback;
            gameProcessor.SaveGamePhaseAsyncCallback = OnSaveGamePhaseAsyncCallback;
            gameProcessor.SaveDecisionAsyncCallback = OnSaveDecisionAsyncCallback;
            gameProcessor.GameViewUpdatedAsyncCallback = OnGameViewUpdatedAsyncCallback;

            await AddGameAsync(game.GameId, gameProcessor).ConfigureAwait(false);

            await gameProcessor.StartAsync().ConfigureAwait(false);

            return true;
        }

        private async Task<bool> OnGameViewUpdatedAsyncCallback(string arg1, string userId, GameView gameView)
        {
            try
            {
                GameViewHub.SendGameViewUpdateAsync(_GameViewHub, userId, gameView);
            }
            catch (Exception ex)
            {

            }

            return true;
        }

        private async Task OnGameFinishedAsync(string gameId)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                gameProcessor.GameFinishedAsyncCallback = null;
                gameProcessor.SaveGameStatusAsyncCallback = null;
                gameProcessor.SaveGamePhaseAsyncCallback = null;
                gameProcessor.SaveDecisionAsyncCallback = null;
                gameProcessor.GameViewUpdatedAsyncCallback = null;

                await RemoveGameAsync(gameId).ConfigureAwait(false);
            }
        }

        private async Task<bool> OnSaveGameStatusAsyncCallback(string gameId, GameStatusEnum gameStatus)
        {
            return await _GameDataAccess.UpdateGameStatusAsync(gameId, gameStatus).ConfigureAwait(false);
        }

        private async Task<bool> OnSaveGamePhaseAsyncCallback(string gameId, string gamePhaseId, int sequence)
        {
            return await _GameDataAccess.UpdateGamePhaseAsync(gameId, gamePhaseId, sequence).ConfigureAwait(false);
        }

        private async Task<bool> OnSaveDecisionAsyncCallback(string gameId, Decision decision)
        {
            return await _GameDataAccess.SaveDecisionAsync(gameId, decision).ConfigureAwait(false);
        }

        public async Task<bool> RemovePlayerFromGameAsync(string gameId, string playerId)
        {
            //Get game
            var game = await _GameDataAccess.GetGameAsync(gameId).ConfigureAwait(false);

            if (game == null)
            {
                // Game doesnt exist
                return false;
            }

            if (game.State != GameStatusEnum.WaitingForPlayers)
            {
                // Game already started or finished
                return false;
            }

            return await _GameDataAccess.RemovePlayerFromGameAsync(gameId, playerId).ConfigureAwait(false);
        }

        public async Task<GameView> GetCurrentGameViewAsync(string gameId, string userId)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                return await gameProcessor.GetCurrentGameViewAsync(userId).ConfigureAwait(false);
            }
            else
            {
                var game = await _GameDataAccess.GetGameAsync(gameId).ConfigureAwait(false);

                if(game != null && game.State == GameStatusEnum.Finished)
                {
                    var gameView = new GameView();

                    if (game.PlayerRoles.TryGetValue(userId, out var role))
                    {
                        gameView.GameRole = role;

                        gameView.GameFinishedView = GameProcessor.BuildGameFinishedView(game);

                        return gameView;
                    }
                }
            }

            return null;
        }

        public async Task<bool> SubmitDecisionAsync(string gameId, string decisionId, User user)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                return await gameProcessor.SubmitDecisionAsync(decisionId, user.UserId).ConfigureAwait(false);
            }

            return false;
        }

        public async Task<bool> StartPhaseClickedAsync(string gameId, User user)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                return await gameProcessor.StartPhaseClickedAsync(user.UserId).ConfigureAwait(false);
            }

            return false;
        }

        public async Task<bool> PhaseDoneClickedAsync(string gameId, User user)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                return await gameProcessor.PhaseDoneClickedAsync(user.UserId).ConfigureAwait(false);
            }

            return false;
        }

        public async Task<bool> CancelGameAsync(string gameId)
        {
            if (TryGetValue(gameId, out var gameProcessor))
            {
                return await gameProcessor.CancelAsync().ConfigureAwait(false);
            }

            return false;
        }

        public async Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId)
        {
            return await _GameDataAccess.GetGamesAsync(userId).ConfigureAwait(false);
        }

        public async Task<string> JoinGameAsync(string accessCode, User user)
        {
            var org = await _OrganizationLogic.GetOrganizationByUserAsync(user.UserId).ConfigureAwait(false);

            if (org == null)
            {
                // Organization unknown
                return null;
            }

            //Get game
            var game = await _GameDataAccess.GetGameByAccessCodeAsync(accessCode).ConfigureAwait(false);

            if (game == null)
            {
                // Game doesnt exist or code is wrong
                return null;
            }

            if (!game.OrganizationId.Equals(org.OrganizationId))
            {
                // User not in the organization
                return null;
            }

            if (game.State != GameStatusEnum.WaitingForPlayers)
            {
                // Game already started or finished
                return null;
            }

            if (game.PlayerIds.Contains(user.UserId))
            {
                // Player already in game
                return null;
            }

            // Double check access code
            if (game.AccessCode.Equals(accessCode, StringComparison.OrdinalIgnoreCase))
            {
                var addPlayerResult = await _GameDataAccess.AddPlayerToGameAsync(game.GameId, user.UserId).ConfigureAwait(false);

                if (addPlayerResult)
                {
                    return game.GameId;
                }
            }

            //Code is wrong
            return null;

        }

        private async Task<bool> GameAlreadyExistsAsync(string gameId)
        {
            await _RunningGameSemaphore.WaitAsync();
            try
            {
                if (_RunningGames.ContainsKey(gameId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                _RunningGameSemaphore.Release();
            }
        }

        private async Task AddGameAsync(string gameId, IGameProcessor gameProcessor)
        {
            await _RunningGameSemaphore.WaitAsync();
            try
            {
                _RunningGames.Add(gameId, gameProcessor);
            }
            finally
            {
                _RunningGameSemaphore.Release();
            }
        }

        private bool TryGetValue(string gameId, out IGameProcessor gameProcessor)
        {
            _RunningGameSemaphore.Wait();
            try
            {
                return _RunningGames.TryGetValue(gameId, out gameProcessor);
            }
            finally
            {
                _RunningGameSemaphore.Release();
            }
        }

        private async Task RemoveGameAsync(string gameId)
        {
            await _RunningGameSemaphore.WaitAsync();
            try
            {
                _RunningGames.Remove(gameId);
            }
            finally
            {
                _RunningGameSemaphore.Release();
            }
        }
    }
}
