using ProductOwnerSimGame.Dtos.GameView;
using System;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Hubs
{
    public interface IGameViewHub
    {
        Task UpdateGameViewAsync(GameView gameView);
    }
}
