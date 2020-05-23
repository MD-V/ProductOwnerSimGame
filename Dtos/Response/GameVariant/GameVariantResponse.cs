using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.Response.GameVariant
{
    public class GameVariantResponse
    {
        
        public IEnumerable<GameVariantDto> GameVariants { get; set; }
    }
}
