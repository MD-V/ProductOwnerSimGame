using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models
{
    public class Game
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
     
        [JsonProperty("game_id")]
        public string GameId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public GameStatusEnum State { get; set; }

        [JsonProperty("curr_phase")]
        public string CurrentGamePhaseId { get; set; }

        [JsonProperty("c_gp_seq")]
        public int CurrentGamePhaseSequence { get; set; }

        [JsonProperty("decs")]
        public List<Decision> Decisions { get; set; } = new List<Decision>();

        [JsonProperty("iv_list")]
        public Dictionary<EffectCategory, int> InitialValues { get; set; } = new Dictionary<EffectCategory, int>();

        [JsonProperty("acc_code")]
        public string AccessCode { get; set; }

        [JsonProperty("gv_id")]
        public string GameVariantId { get; set; }

        [JsonProperty("org_id")]
        public string OrganizationId { get; set; }

        [JsonProperty("player_roles")]
        public Dictionary<string, GameRole> PlayerRoles { get; set; } = new Dictionary<string, GameRole>();

        [JsonProperty("player_ids")]
        public List<string> PlayerIds { get; set; } = new List<string>();

        [JsonProperty("gm_id")]
        public string GameMasterId { get; set; }
    }
}
