using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Response.Organization
{
    public class CreateOrganizationResponse
    {
        [JsonProperty("org_id")]
        public string OrganizationId { get; set; }

        public CreateOrganizationResponse(string organizationId)
        {
            OrganizationId = organizationId;
        }
    }
}
