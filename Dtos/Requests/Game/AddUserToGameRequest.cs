using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Dtos.Requests.Game
{
    public class AddUserToGameRequest
    {
        [JsonProperty("game_id")]
        public string GameId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("acc_code")]
        public string AccessCode { get; set; }
    }
}
