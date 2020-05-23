using ProductOwnerSimGame.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProductOwnerSimGame.Models.Roles
{
    public static class IntegratedRoles
    {
        public static List<Role> All()
        {
            return new List<Role>
            {
                Admin,
                OrgAdmin,
                User,
                GameMaster,
            };
        }

        public static readonly Role Admin = new Role
        {
            Id = new Guid("882F54C7-6B3B-4981-AAC8-37AD8A92CA7B"),
            Name = RoleNameForAdmin,
            NormalizedName = RoleNameForAdmin.ToUpper(CultureInfo.InvariantCulture),
            IsSystemDefault = true,
            RolePermissions = new List<Permission>
            {
                IntegratedPermissions.UserAccess,
                IntegratedPermissions.GameMasterAccess,
                IntegratedPermissions.OrgAdministrationAccess,
                IntegratedPermissions.AdministrationAccess
            }
        };

        public static readonly Role OrgAdmin = new Role
        {
            Id = new Guid("168A4E08-5506-4D2A-BDCB-4390EA4A4DE0"),
            Name = RoleNameForOrgAdmin,
            NormalizedName = RoleNameForOrgAdmin.ToUpper(CultureInfo.InvariantCulture),
            IsSystemDefault = true,
            RolePermissions = new List<Permission>
            {
                IntegratedPermissions.GameMasterAccess,
                IntegratedPermissions.UserAccess,
                IntegratedPermissions.OrgAdministrationAccess,
            }
        };

        public static readonly Role User = new Role
        {
            Id = new Guid("B296E0FE-C67C-4F77-A07A-5791730963C9"),
            Name = RoleNameForUser,
            NormalizedName = RoleNameForUser.ToUpper(CultureInfo.InvariantCulture),
            IsSystemDefault = true,
            RolePermissions = new List<Permission>
            {
                IntegratedPermissions.UserAccess
            }
        };

        public static readonly Role GameMaster = new Role
        {
            Id = new Guid("CC5D1E61-2AB3-48CD-9207-643B4EE764A7"),
            Name = RoleNameForGameMaster,
            NormalizedName = RoleNameForGameMaster.ToUpper(CultureInfo.InvariantCulture),
            IsSystemDefault = true,
            RolePermissions = new List<Permission>
            { 
                IntegratedPermissions.GameMasterAccess,
                IntegratedPermissions.UserAccess
            }
        };

        private const string RoleNameForAdmin = "Admin";
        private const string RoleNameForOrgAdmin = "OrgAdmin";
        private const string RoleNameForUser = "User";
        private const string RoleNameForGameMaster = "GameMaster";
    }
}
