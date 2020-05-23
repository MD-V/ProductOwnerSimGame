using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.GameVariant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.DataAccess.Interfaces
{
    public interface IGameDataAccess
    {
        Task<Game> CreateNewGameAsync(GameVariant gameVariant, string organizationId, string gameMasterUserId);
        
        Task<bool> StartGameAsync(string gameId);
        
        Task<Game> GetGameAsync(string gameId);

        Task<bool> AddPlayerToGameAsync(string gameId, string userId);

        Task<bool> AssignRoleToPlayerAsync(string gameId, string userId, GameRole gameRole);

        Task<bool> RemovePlayerFromGameAsync(string gameId, string playerId);
        
        Task<bool> UpdateGameStatusAsync(string gameId, GameStatusEnum gameStatus);
        
        Task<bool> UpdateGamePhaseAsync(string gameId, string gamePhaseId, int sequence);
        
        Task<bool> SaveDecisionAsync(string gameId, Decision decision);
        
        Task<IReadOnlyCollection<Game>> GetGamesAsync(string userId);
        
        Task<Game> GetGameByAccessCodeAsync(string accessCode);
        
        Task<IReadOnlyCollection<string>> GetRunningGameIdsAsync();
        
        Task<bool> ClearRolesAsync(string gameId);
    }
}
