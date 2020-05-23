using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Dtos.Requests.Game;
using ProductOwnerSimGame.Dtos.Response.Game;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameLogic _GameLogic;
        private readonly IGameVariantLogic _GameVariantLogic;
        private readonly IUserLogic _UserLogic;
        private readonly IOrganizationLogic _OrganizationLogic;
        private readonly IMapper _Mapper;

        public GameController(IGameLogic gameLogic, IUserLogic userLogic, IOrganizationLogic organizationLogic, IGameVariantLogic gameVariantLogic, IMapper mapper)
        {
            _GameLogic = gameLogic;
            _GameVariantLogic = gameVariantLogic;
            _UserLogic = userLogic;
            _OrganizationLogic = organizationLogic;
            _Mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<IActionResult> GetGames()
        {
            //TODO User Kontext
            var user = await _UserLogic.GetUserAsync(HttpContext.User.Identity.Name).ConfigureAwait(false);

            var games = await _GameLogic.GetGamesAsync(user.UserId).ConfigureAwait(false);

            var gamesDto = _Mapper.Map<IEnumerable<GameDto>>(games);

            foreach (var game in gamesDto)
            {
                try
                {
                    var gameVariant = await _GameVariantLogic.GetGameVariantAsync(game.GameVariantId).ConfigureAwait(false);

                    if (gameVariant != null)
                    {
                        game.GameVariantName = gameVariant.DisplayName;
                        game.GameVariantPlayerCount = gameVariant.PlayerCount;
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return Ok(gamesDto);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame(string gameId)
        {
            return Ok(await _GameLogic.GetGameAsync(gameId).ConfigureAwait(false));
        }

        [HttpPost]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForGameMasters)]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest createGameRequest)
        {
            var user = await _UserLogic.GetUserAsync(HttpContext.User.Identity.Name).ConfigureAwait(false);

            var org = await _OrganizationLogic.GetOrganizationByUserAsync(user.UserId).ConfigureAwait(false);

            var game = await _GameLogic.CreateNewGameAsync(createGameRequest.GameVariantId, org.OrganizationId, user.UserId).ConfigureAwait(false);

            var response = new CreateGameResponse(game.GameId, game.AccessCode);


            return Ok(response);
        }

        [HttpPost("{gameId}/start")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForGameMasters)]
        public async Task<ActionResult<StartGameResponse>> StartGame(string gameId)
        {
            var startGame = await _GameLogic.StartGameAsync(gameId).ConfigureAwait(false);

            var response = new StartGameResponse()
            {
                Started = startGame
            };

            return Ok(response);
        }

        [HttpPost("{gameId}/cancel")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForGameMasters)]
        public async Task<ActionResult<CancelGameResponse>> CancelGame(string gameId)
        {
            var cancelGame = await _GameLogic.CancelGameAsync(gameId).ConfigureAwait(false);

            var response = new CancelGameResponse()
            {
                Cancelled = cancelGame
            };


            return Ok(response);
        }

        [HttpPost("{gameId}/startphase")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<IActionResult> StartPhaseClickedAsync(string gameId)
        {
            return Ok(await _GameLogic.StartPhaseClickedAsync(gameId, HttpContext.User.Identity.Name).ConfigureAwait(false));
        }

        [HttpPost("{gameId}/phasedone")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<IActionResult> PhaseDoneClickedAsync(string gameId)
        {
            return Ok(await _GameLogic.PhaseDoneClickedAsync(gameId, HttpContext.User.Identity.Name).ConfigureAwait(false));
        }

        [HttpPost("{gameId}/submitdecision/{decisionId}")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<IActionResult> SubmitDecision(string gameId, string decisionId)
        {
            return Ok(await _GameLogic.SubmitDecision(gameId, decisionId, HttpContext.User.Identity.Name).ConfigureAwait(false));
        }

        [HttpPost("RemoveUser")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<IActionResult> RemovePlayer([FromBody] RemoveUserFromGameRequest removeUserFromGameRequest)
        {
            return Ok(await _GameLogic.RemovePlayerFromGameAsync(removeUserFromGameRequest.GameId, removeUserFromGameRequest.UserId).ConfigureAwait(false));
        }

        [HttpPost("join")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForUserAccess)]
        public async Task<ActionResult<JoinGameResponse>> Join([FromBody] JoinGameRequest addUserToGameRequest)
        {
            var gameId = await _GameLogic.JoinGameAsync(addUserToGameRequest.AccessCode, HttpContext.User.Identity.Name).ConfigureAwait(false);

            if (string.IsNullOrEmpty(gameId))
            {
                return BadRequest();
            }

            var response = new JoinGameResponse()
            {
                GameId = gameId
            };

            return Ok(response);
        }
    }
}
