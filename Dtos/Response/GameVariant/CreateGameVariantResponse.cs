using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Response.GameVariant
{
    public class CreateGameVariantResponse
    {
        [JsonProperty("gv_id")]
        public string GameVariantId { get; set; }

        public CreateGameVariantResponse(string gameVariantId)
        {
            GameVariantId = gameVariantId;
        }
    }
}
