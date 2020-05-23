using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models
{
    public class Decision
    {
        [JsonProperty("dec_id")]
        public string DecisionId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("dec_md_txt")]
        public string DecisionMarkdownText { get; set; }

        [JsonProperty("effs")]
        public List<Effect> Effects { get; set; } = new List<Effect>();

        [JsonProperty("exp_md_txt")]
        public string ExplanationMarkdownText { get; set; }
    }
}