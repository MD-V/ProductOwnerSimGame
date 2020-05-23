using System;
using Microsoft.AspNetCore.Identity;
using ProductOwnerSimGame.Models.Roles;

namespace ProductOwnerSimGame.Models.Users
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
