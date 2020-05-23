using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Requests.Game
{
    public class SubmitDecisionRequest
    {
       
        [JsonProperty("dec_id")]
        public string DecisionId { get; set; }
    }
}
