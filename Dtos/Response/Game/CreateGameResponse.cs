using Newtonsoft.Json;

namespace ProductOwnerSimGame.Dtos.Response.Game
{
    public class CreateGameResponse
    {
       
        public string GameId { get; set; }

        
        public string AccessCode { get; set; }

        public CreateGameResponse(string gameId, string accessCode)
        {
            GameId = gameId;
            AccessCode = accessCode;
        }
    }
}
