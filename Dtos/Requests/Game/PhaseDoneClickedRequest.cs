using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductOwnerSimGame.Dtos.Requests.Game
{
    public class PhaseDoneClickedRequest
    {
        
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}
