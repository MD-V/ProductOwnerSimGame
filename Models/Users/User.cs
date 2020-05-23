using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ProductOwnerSimGame.Models.Users
{
    public class User : IdentityUser<Guid>
    {
        [JsonProperty("id")]
        public string DocumentId { get; set; } = Guid.NewGuid().ToString();

        public string UserId => Id.ToString();

        public string UserRoleId { get; set; }
    }
}