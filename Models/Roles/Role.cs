using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProductOwnerSimGame.Models.Permissions;

namespace ProductOwnerSimGame.Models.Roles
{
    public class Role : IdentityRole<Guid>
    {
        public bool IsSystemDefault { get; set; } = false;

        public virtual ICollection<Permission> RolePermissions { get; set; } = new List<Permission>();

        public string RoleId => Id.ToString();
    }
}
