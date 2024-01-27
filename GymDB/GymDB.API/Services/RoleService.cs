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

        public bool HasUserRole(User user, string role)
            => user.Role.NormalizedName == role;

        public bool HasUserAnyRole(User user, string[] roles)
            => roles.Contains(user.Role.NormalizedName);

        public bool AssignUserRole(User user, string role)
        {
            Role? foundRole = GetRoleByNormalizedName(role);

            if (foundRole == null)
                return false;

            user.RoleId = foundRole.Id;
            user.Role = foundRole;

            return true;
        }
    }
}
