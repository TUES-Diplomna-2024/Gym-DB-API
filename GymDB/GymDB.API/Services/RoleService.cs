using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationContext context;

        public RoleService(ApplicationContext context)
        {
            this.context = context;
        }

        public Role? GetRoleByNormalizedName(string normalizedName)
            => context.Roles.FirstOrDefault(role => role.NormalizedName == normalizedName);

        public bool HasUserAnyRole(User user, string[] roles)
            => roles.Contains(user.Role.NormalizedName);

        public bool AssignUserRole(User user, string roleNormalizedName)
        {
            Role? role = GetRoleByNormalizedName(roleNormalizedName);

            if (role == null)
                return false;

            user.RoleId = role.Id;
            user.Role = role;

            return true;
        }
    }
}
