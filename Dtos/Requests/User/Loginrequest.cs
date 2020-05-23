using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Requests.User
{
    public class Loginrequest
    {
        [JsonProperty]
        public string UserNameOrEmail { get; set; }

        [JsonProperty]
        public string Password { get; set; }
    }
}
