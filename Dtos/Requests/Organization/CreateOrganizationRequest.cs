using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Requests.Organization
{
    public class CreateOrganizationRequest
    {

        [JsonProperty("disp_name")]
        public string DisplayName { get; set; }



    }
}
