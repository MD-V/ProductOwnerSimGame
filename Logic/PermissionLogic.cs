using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Logic;
using ProductOwnerSimGame.Models.Permissions;
using ProductOwnerSimGame.Models.Roles;
using ProductOwnerSimGame.Models.Users;

namespace ProductOwnerSimGame.Permissions
{
    public class PermissionLogic : IPermissionLogic
    {
        private readonly UserManager<User> _UserManager;

        private readonly IMapper _Mapper;

        public PermissionLogic(
            UserManager<User> userManager,
            IMapper mapper)
        {
            _UserManager = userManager;
            _Mapper = mapper;
        }

        public async Task<IEnumerable<PermissionDto>> GetGrantedPermissionsAsync(string userNameOrEmail)
        {
           
            var user = _UserManager.Users.FirstOrDefault(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail);

            var userRole = user?.UserRoleId;


            var integratedRole = IntegratedRoles.All().FirstOrDefault(a => a.RoleId.Equals(userRole));


            return _Mapper.Map<IEnumerable<PermissionDto>>(integratedRole.RolePermissions);
        }

        public async Task<bool> IsUserGrantedToPermissionAsync(string userNameOrEmail, string permissionName)
        {
            var user = _UserManager.Users.FirstOrDefault(u =>
                u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
            if (user == null)
            {
                return false;
            }

            var userRole = user?.UserRoleId;


            var integratedRole = IntegratedRoles.All().FirstOrDefault(a => a.RoleId.Equals(userRole));

            var grantedPermissions = integratedRole.RolePermissions;

            return grantedPermissions.Any(p => p.Name == permissionName);
        }

        public void InitializePermissions(List<Permission> permissions)
        {
            /*
            _dbContext.RolePermissions.RemoveRange(_dbContext.RolePermissions.Where(rp => rp.RoleId == DefaultRoles.Admin.Id));
            _dbContext.SaveChanges();

            _dbContext.Permissions.RemoveRange(_dbContext.Permissions);
            _dbContext.SaveChanges();

            _dbContext.AddRange(permissions);
            GrantAllPermissionsToAdminRole(permissions);
            _dbContext.SaveChanges();
            */
        }

        private void GrantAllPermissionsToAdminRole(List<Permission> permissions)
        {
            /*
            foreach (var permission in permissions)
            {
                _dbContext.RolePermissions.Add(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = DefaultRoles.Admin.Id
                });
            }
            */
        }
    }
}
