using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models
{
    public class Organization
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("org_id")]
        public string OrganizationId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("usr_ids")]
        public List<string> UserIds { get; set; } = new List<string>();


        [JsonProperty("disp_name")]
        public string DisplayName { get; set; }
    }
}
