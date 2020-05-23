using System;
using System.Globalization;
using System.Collections.Generic;
using ProductOwnerSimGame.Models.Roles;

namespace ProductOwnerSimGame.Models.Users
{
    public class IntegratedUsers
    {
        public static List<User> All()
        {
            return new List<User>
            {
                Admin,
                OrgAdmin,
                GameMaster,
                User1,
                User2,
                User3,
            };
        }

        public static readonly User Admin = new User
        {
            Id = new Guid("C41A7761-6645-4E2C-B99D-F9E767B9AC77"),
            UserName = AdminUserName,
            Email = AdminUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = AdminUserEmail.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = AdminUserName.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,   
            UserRoleId = IntegratedRoles.Admin.RoleId
        };

        public static readonly User OrgAdmin = new User
        {
            Id = new Guid("4B6D9E45-626D-489A-A8CF-D32D36583AF4"),
            UserName = OrgAdminUserName,
            Email = OrgAdminUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = OrgAdminUserEmail.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = OrgAdminUserName.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,
            UserRoleId = IntegratedRoles.OrgAdmin.RoleId
        };

        public static readonly User User1 = new User
        {
            Id = new Guid("065E903E-6F7B-42B8-B807-0C4197F9D1BC"),
            UserName = User1Name,
            Email = User1Email,
            EmailConfirmed = true,
            NormalizedEmail = User1Email.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = User1Name.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,
            UserRoleId = IntegratedRoles.User.RoleId
        };

        public static readonly User User2 = new User
        {
            Id = new Guid("69105B2C-AC1F-4290-BE45-8D45BE2FF14D"),
            UserName = User2Name,
            Email = User2Email,
            EmailConfirmed = true,
            NormalizedEmail = User2Email.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = User2Name.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,
            UserRoleId = IntegratedRoles.User.RoleId
        };

        public static readonly User User3 = new User
        {
            Id = new Guid("E35131A2-1CE3-47B7-810D-D8FC25F0F383"),
            UserName = User3Name,
            Email = User3Email,
            EmailConfirmed = true,
            NormalizedEmail = User3Email.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = User3Name.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,
            UserRoleId = IntegratedRoles.User.RoleId
        };

        public static readonly User GameMaster = new User
        {
            Id = new Guid("4C294ED4-F8FC-4426-8B6A-76E11BCC77C1"),
            UserName = GameMasterUserName,
            Email = GameMasterUserEmail,
            EmailConfirmed = true,
            NormalizedEmail = GameMasterUserEmail.ToUpper(CultureInfo.InvariantCulture),
            NormalizedUserName = GameMasterUserName.ToUpper(CultureInfo.InvariantCulture),
            AccessFailedCount = 5,
            PasswordHash = PasswordHashFor123Qwe,
            UserRoleId = IntegratedRoles.GameMaster.RoleId
        };

        private const string AdminUserName = "admin";
        private const string AdminUserEmail = "admin@mail.com";
        private const string OrgAdminUserName = "orgadmin";
        private const string OrgAdminUserEmail = "orgadmin@mail.com";
        private const string PasswordHashFor123Qwe = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw=="; //123qwe
        private const string User1Name = "user1";
        private const string User1Email = "user1@mail.com";
        private const string User2Name = "user2";
        private const string User2Email = "user2@mail.com";
        private const string User3Name = "user3";
        private const string User3Email = "user3@mail.com";
        private const string GameMasterUserName = "gamemaster";
        private const string GameMasterUserEmail = "gamemaster@mail.com";
    }
}
