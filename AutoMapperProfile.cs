using AutoMapper;
using ProductOwnerSimGame.Dtos;
using ProductOwnerSimGame.Dtos.Response.Role;
using ProductOwnerSimGame.Dtos.Response.User;
using ProductOwnerSimGame.Models;
using ProductOwnerSimGame.Models.GameVariant;
using ProductOwnerSimGame.Models.Permissions;
using ProductOwnerSimGame.Models.Roles;
using ProductOwnerSimGame.Models.Users;

namespace ProductOwnerSimGame
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(u => u.Password, opt => opt.Ignore());

            CreateMap<User, UserListResponse>();
            CreateMap<Permission, PermissionDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<Role, RoleListResponse>();
            CreateMap<GameVariant, GameVariantDto>();
            CreateMap<Decision, DecisionDto>();

            CreateMap<Game, GameDto>()
                .ForMember(d => d.CurrentJoinedPlayers, o => o.MapFrom(s => s.PlayerIds.Count));
        }
    }
}