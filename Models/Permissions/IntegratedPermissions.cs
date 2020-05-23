using System;
using System.Collections.Generic;

namespace ProductOwnerSimGame.Models.Permissions
{
    internal class IntegratedPermissions
    {
        internal static List<Permission> All()
        {
            return new List<Permission>
            {
                AdministrationAccess,
                OrgAdministrationAccess,
                GameMasterAccess,
                UserAccess,
            };
        }


        internal static readonly Permission AdministrationAccess = new Permission
        {
            DisplayName = "Administration access",
            Name = PermissionNameForAdministration,
            Id = new Guid("2A1CCB43-FA4F-48CE-B601-D3AB4D611B32")
        };

        internal static readonly Permission OrgAdministrationAccess = new Permission
        {
            DisplayName = "OrgAdministration access",
            Name = PermissionNameForOrgAdministration,
            Id = new Guid("6EA019A4-DFBD-4692-A94F-4C5CE69AC77F")
        };

        internal static readonly Permission GameMasterAccess = new Permission
        {
            DisplayName = "GameMaster access",
            Name = PermissionNameForGameMasters,
            Id = new Guid("28126FFD-51C2-4201-939C-B64E3DF43B9D")
        };

        internal static readonly Permission UserAccess = new Permission
        {
            DisplayName = "User access",
            Name = PermissionNameForUserAccess,
            Id = new Guid("86D804BD-D022-49A5-821A-D2240478AAC4")
        };

        

        // these strings are using on authorize attributes

        internal const string PermissionNameForUserAccess = "Permissions_User";
        internal const string PermissionNameForAdministration = "Permissions_Administration";
        internal const string PermissionNameForOrgAdministration = "Permissions_OrgAdministration";
        internal const string PermissionNameForGameMasters = "Permissions_GameMaster";

        

        // add your permission names
    }
}
