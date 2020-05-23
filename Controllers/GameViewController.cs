using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;
using System.Threading.Tasks;
using ProductOwnerSimGame.Dtos.GameView;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameViewController : ControllerBase
    {
        private IGameViewLogic _GameStateLogic;

        public GameViewController(IGameViewLogic gameStateLogic)
        {
            _GameStateLogic = gameStateLogic;
        }

        // GET: api/GameView/5
        [HttpGet("{gameId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<ActionResult<GameView>> GetCurrentGameViewAsync(string gameId)
        {
            return Ok(await _GameStateLogic.GetCurrentGameViewAsync(gameId, HttpContext.User.Identity.Name).ConfigureAwait(false));
        }
    }
}
