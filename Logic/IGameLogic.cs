using ProductOwnerSimGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IGameLogic
    {
        Task<Game> CreateNewGameAsync(string gameVariantId, string organizationId, string gameMasterUserId);

        Task<Game> GetGameAsync(string gameId);

        Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId);

        Task<bool> StartGameAsync(string gameId);

        Task<bool> CancelGameAsync(string gameId);

        Task<bool> RemovePlayerFromGameAsync(string gameId, string userId);

        Task<bool> StartPhaseClickedAsync(string gameId, string userName);
        
        Task<bool> PhaseDoneClickedAsync(string gameId, string userName);
        
        Task<bool> SubmitDecision(string gameId, string decisionId, string userName);
        
        Task<string> JoinGameAsync(string accessCode, string userName);
    }
}