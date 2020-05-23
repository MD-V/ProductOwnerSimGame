using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Dtos.Requests.GameVariant;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameVariantController : ControllerBase
    {
        private readonly IGameVariantLogic _GameVariantLogic;
        private readonly IUserLogic _UserLogic;
        private readonly IOrganizationLogic _OrganizationLogic;
        private readonly IMapper _Mapper;

        public GameVariantController(IGameVariantLogic gameStateLogic, IUserLogic userLogic, IOrganizationLogic organizationLogic, IMapper mapper)
        {
            _GameVariantLogic = gameStateLogic;
            _UserLogic = userLogic;
            _OrganizationLogic = organizationLogic;
            _Mapper = mapper;
        }

        [HttpPost("search")]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForGameMasters)]
        public async Task<IActionResult> GetGameVariants([FromBody] GetGameVariantsRequest getGameVariantsRequest)
        {
            var user = await _UserLogic.GetUserAsync(HttpContext.User.Identity.Name).ConfigureAwait(false);

            var org = await _OrganizationLogic.GetOrganizationByUserAsync(user.UserId).ConfigureAwait(false);

            var gameVariants = await _GameVariantLogic.GetGameVariantsAsync(org?.OrganizationId, getGameVariantsRequest.PlayerCount).ConfigureAwait(false);

            var gameVariantsDto = _Mapper.Map<IEnumerable<GameVariantDto>>(gameVariants);

            return Ok(gameVariantsDto);
        }


        [HttpPost]
        [Authorize(Policy = IntegratedPermissions.PermissionNameForAdministration)]
        public async Task<IActionResult> CreateGameVariant()
        {
            // TODO create game variant
            throw new NotImplementedException();
            

            
        }
    }
}
