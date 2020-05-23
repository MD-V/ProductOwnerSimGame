using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProductOwnerSimGame.Models;

namespace ProductOwnerSimGame.Dtos
{
    public class GameDto
    {
        public string GameId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GameStatusEnum State { get; set; }

        public string AccessCode { get; set; }

        [JsonIgnore]
        public string GameVariantId { get; set; }

        public string GameVariantName { get; set; }

        public int GameVariantPlayerCount { get; set; }
        
        
        public int CurrentJoinedPlayers { get; set; }
    }
}
