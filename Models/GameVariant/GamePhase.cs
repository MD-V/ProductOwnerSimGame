using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models.GameVariant
{
    public class GamePhase
    {
        [JsonProperty("gp_id")]
        public string GamePhaseId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("sequence")]
        public int Sequence { get; set; }

        [JsonProperty("po_miss")]
        public Mission ProductOwnerMission { get; set; }

        [JsonProperty("sh_miss")]
        public List<Mission> StakeholderMissions { get; set; } = new List<Mission>();

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("pd")]
        public PhaseDuration Duration { get; set; }

        [JsonProperty("pd_secs")]
        public int DurationInSeconds { get; set; }

        [JsonProperty("decs")]
        public List<Decision> Decisions { get; set; } = new List<Decision>();
    }
}
