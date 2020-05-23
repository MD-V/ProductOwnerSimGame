using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Simulation
{
    public interface IGameManager
    {
        Task Start();

        Task<bool> StartGameAsync(string gameId);

        Task<Game> CreateNewGameAsync(string gameVariantId, string organizationId, string gameMasterUserId);

        Task<bool> RemovePlayerFromGameAsync(string gameId, string playerId);
        
        Task<bool> CancelGameAsync(string gameId);
        
        Task<GameView> GetCurrentGameViewAsync(string gameId, string userId);

        Task<bool> SubmitDecisionAsync(string gameId, string decisionId, User user);

        Task<bool> StartPhaseClickedAsync(string gameId, User user);

        Task<bool> PhaseDoneClickedAsync(string gameId, User user);
        
        Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId);
        
        Task<string> JoinGameAsync(string accessCode, User user);
    }
}
