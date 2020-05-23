using ProductOwnerSimGame.Models.GameVariant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IGameVariantLogic
    {
        Task<IReadOnlyCollection<GameVariant>> GetGameVariantsAsync(string organizationId, int playerCount = 0);

        Task<string> CreateGameVariantAsync(GameVariant gameVariant);
        Task<GameVariant> GetGameVariantAsync(string gameVariantId);
    }
}
