using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Simulation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public class GameLogic : IGameLogic
    {
        private IGameManager _GameController;
        private IUserLogic _UserLogic;

        public GameLogic(IGameManager gameController, IUserLogic userLogic)
        {
            _GameController = gameController;
            _UserLogic = userLogic;
        }

        public async Task<bool> CancelGameAsync(string gameId)
        {
            return await _GameController.CancelGameAsync(gameId).ConfigureAwait(false);
        }

        public async Task<Game> CreateNewGameAsync(string gameVariantId, string organizationId, string gameMasterUserId)
        {
            return await _GameController.CreateNewGameAsync(gameVariantId, organizationId, gameMasterUserId).ConfigureAwait(false);
        }

        public async Task<string> JoinGameAsync(string accessCode, string userName)
        {
            var user = await _UserLogic.GetUserAsync(userName).ConfigureAwait(false);

            if (user == null)
            {
                // User unknown
                return null;
            }

            return await _GameController.JoinGameAsync(accessCode, user).ConfigureAwait(false);
        }

        public Task<Game> GetGameAsync(string gameId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId)
        {
            return await _GameController.GetGamesAsync(userId).ConfigureAwait(false);
        }

        public async Task<bool> PhaseDoneClickedAsync(string gameId, string userName)
        {
            var user = await _UserLogic.GetUserAsync(userName).ConfigureAwait(false);

            if (user == null)
            {
                // User unknown
                return false;
            }

            return await _GameController.PhaseDoneClickedAsync(gameId, user).ConfigureAwait(false);
        }

        public async Task<bool> StartGameAsync(string gameId)
        {
            return await _GameController.StartGameAsync(gameId).ConfigureAwait(false);
        }

        public async Task<bool> StartPhaseClickedAsync(string gameId, string userName)
        {
            var user = await _UserLogic.GetUserAsync(userName).ConfigureAwait(false);

            if (user == null)
            {
                // User unknown
                return false;
            }

            return await _GameController.StartPhaseClickedAsync(gameId, user).ConfigureAwait(false);
        }

        public async Task<bool> SubmitDecision(string gameId, string decisionId, string userName)
        {
            var user = await _UserLogic.GetUserAsync(userName).ConfigureAwait(false);

            if (user == null)
            {
                // User unknown
                return false;
            }

            return await _GameController.SubmitDecisionAsync(gameId, decisionId, user).ConfigureAwait(false);
        }

        public async Task<bool> RemovePlayerFromGameAsync(string gameId, string playerId)
        {
            return await _GameController.RemovePlayerFromGameAsync(gameId, playerId).ConfigureAwait(false);
        }
    }
}
