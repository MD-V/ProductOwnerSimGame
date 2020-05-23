using ProductOwnerSimGame.DataAccess.Interfaces;
using ProductOwnerSimGame.Models.GameVariant;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public class GameVariantLogic : IGameVariantLogic
    {
        private IGameVariantDataAccess _GameVariantDataAccess;

        public GameVariantLogic(IGameVariantDataAccess gameVariantDataAccess)
        {

            _GameVariantDataAccess = gameVariantDataAccess;
            
        }

        public async Task<string> CreateGameVariantAsync(GameVariant gameVariant)
        {
            return await _GameVariantDataAccess.CreateGameVariantAsync(gameVariant).ConfigureAwait(false);
        }

        public async Task<GameVariant> GetGameVariantAsync(string gameVariantId)
        {
            return await _GameVariantDataAccess.GetGameVariantAsync(gameVariantId).ConfigureAwait(false);
        }

        public async Task<IReadOnlyCollection<GameVariant>> GetGameVariantsAsync(string organizationId, int playerCount)
        {
            return await _GameVariantDataAccess.GetGameVariantsAsync(organizationId, playerCount).ConfigureAwait(false);
        }
    }
}
