using Newtonsoft.Json;

namespace ProductOwnerSimGame.Models
{
    public class Mission
    {
        [JsonProperty("md_txt")]
        public string MarkdownText { get; set; }
    }
}
