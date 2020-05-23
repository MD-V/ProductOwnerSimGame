using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Simulation;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public class GameViewLogic : IGameViewLogic
    {
        private IGameManager _GameController;
        private IUserLogic _UserLogic;

        public GameViewLogic(IGameManager gameController, IUserLogic userLogic)
        {
            _GameController = gameController;
            _UserLogic = userLogic;
        }

        public async Task<GameView> GetCurrentGameViewAsync(string gameId, string userNameOrEmail)
        {
            var user = await _UserLogic.GetUserAsync(userNameOrEmail).ConfigureAwait(false);

            return await _GameController.GetCurrentGameViewAsync(gameId, user.UserId).ConfigureAwait(false);
        }
    }
}
