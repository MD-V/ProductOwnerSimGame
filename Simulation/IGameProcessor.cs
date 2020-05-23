using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Models;
using System;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Simulation
{
    public interface IGameProcessor
    {
        Task StartAsync();

        Task<bool> CancelAsync();

        Task<bool> SubmitDecisionAsync(string decisionId, string userId);

        Task<bool> StartPhaseClickedAsync(string userId);

        Task<bool> PhaseDoneClickedAsync(string userId);

        Task<GameView> GetCurrentGameViewAsync(string userId);

        Func<string, Task> GameFinishedAsyncCallback { get; set; }
        
        Func<string, string, GameView, Task<bool>> GameViewUpdatedAsyncCallback { get; set; }

        Func<string, GameStatusEnum, Task<bool>> SaveGameStatusAsyncCallback { get; set; }

        Func<string, string, int, Task<bool>> SaveGamePhaseAsyncCallback { get; set; }

        Func<string, Decision, Task<bool>> SaveDecisionAsyncCallback { get; set; }
    }
}
