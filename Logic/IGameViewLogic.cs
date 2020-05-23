using ProductOwnerSimGame.Dtos.GameView;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Logic
{
    public interface IGameViewLogic
    {
        Task<GameView> GetCurrentGameViewAsync(string gameId, string userNameOrEmail);
    }
}
