using System.Collections.Generic;

namespace ProductOwnerSimGame.Dtos.Response.User
{
    public class UserResponse
    {
        public string Username { get; set; }
        public string EMail { get; set; }
        public string UserId { get; set; }

        public IEnumerable<string> Roles { get; set; } 
    }
}
