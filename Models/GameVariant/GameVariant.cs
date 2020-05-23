using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models.GameVariant
{
    public class GameVariant
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("gv_id")]
        public string GameVariantId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("disp_name")]
        public string DisplayName { get; set; }

        [JsonProperty("gp_list")]
        public List<GamePhase> GamePhases { get; set; } = new List<GamePhase>();

        [JsonProperty("iv_list")]
        public Dictionary<EffectCategory, int> InitialProjectValues { get; set; } = new Dictionary<EffectCategory, int>();

        [JsonProperty("ov_md_txt")]
        public string OverallMarkDownText { get; set; }

        [JsonProperty("public")]
        public bool IsPublic { get; set; }

        [JsonProperty("pl_count")]
        public int PlayerCount { get; set; }

        [JsonProperty("org_id")]
        public string OrganizationId { get; set; }

        [JsonIgnore]
        public Organization Organization { get; set; }
    }
}
