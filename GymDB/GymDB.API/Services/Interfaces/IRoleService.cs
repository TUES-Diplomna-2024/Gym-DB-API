using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;

namespace GymDB.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task EnsureRolesCreatedAsync();

        Task EnsureRootAdminCreatedAsync();

        Task AssignUserDefaultRoleAsync(User user);

        Task AssignUserNewRoleAsync(HttpContext context, Guid userId, AssignableRole role);

        bool HasUserRole(User user, string role);

        bool HasUserAnyRole(User user, string[] roles);
    }
}
