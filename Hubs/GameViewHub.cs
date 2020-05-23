using Microsoft.AspNetCore.SignalR;
using ProductOwnerSimGame.Dtos.GameView;
using ProductOwnerSimGame.Logic;
using System;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Hubs
{

    public class GameViewHub : Hub<IGameViewHub>
    {
        private readonly static ConnectionMapping<string> _connections =
             new ConnectionMapping<string>();
        
        private IUserLogic _UserLogic;

        public GameViewHub(IUserLogic userLogic)
        {
            _UserLogic = userLogic;
        }

        public override async Task OnConnectedAsync()
        {
            var user = await _UserLogic.GetUserAsync(Context.User.Identity.Name).ConfigureAwait(false);

            if (user == null)
            {
                return;
            }

            _connections.Add(user.UserId, Context.ConnectionId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var user = await _UserLogic.GetUserAsync(Context.User.Identity.Name).ConfigureAwait(false);

            if(user == null)
            {
                return;
            }

            _connections.Remove(user.UserId, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public static async Task SendGameViewUpdateAsync(IHubContext<GameViewHub, IGameViewHub> context, string userId, GameView gameView)
        {
            foreach (var connectionId in _connections.GetConnections(userId))
            {
               context.Clients.Client(connectionId).UpdateGameViewAsync(gameView);
            }
        }
    }
}
