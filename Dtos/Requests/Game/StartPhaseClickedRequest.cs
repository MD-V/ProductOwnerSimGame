using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Requests.Game
{
    public class StartPhaseClickedRequest
    {
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
