using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationContext context;
        private readonly IUserService userService;

        public RoleService(ApplicationContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public Role? GetByNormalizedName(string normalizedName)
            => context.Roles.FirstOrDefault(role => role.NormalizedName == normalizedName);

        public bool AssignUserRole(User user, string roleName)
        {
            Role? role = GetByNormalizedName(roleName.ToUpper());

            if (role == null)
                return false;

            user.RoleId = role.Id;
            user.Role = role;

            return true;
        }
    }
}
