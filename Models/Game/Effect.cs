using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProductOwnerSimGame.Models
{
    public class Effect
    {
        [JsonProperty("eff_cat")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EffectCategory EffectCategory { get; set; }

        public int Value { get; set; }
    }
}