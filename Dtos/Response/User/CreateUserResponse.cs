using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Response.User
{
    public class CreateUserResponse
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        public CreateUserResponse(string userId)
        {
            UserId = userId;
        }
    }
}
