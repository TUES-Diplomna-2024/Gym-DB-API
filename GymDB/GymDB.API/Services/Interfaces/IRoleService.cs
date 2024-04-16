using GymDB.API.Data.Entities;
using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IRoleService
    {
        Task EnsureRolesCreatedAsync();

        Task EnsureRootAdminCreatedAsync();

        Task AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel);

        Task AssignUserDefaultRoleAsync(User user);

        bool HasUserRole(User user, string role);

        bool HasUserAnyRole(User user, string[] roles);
    }
}
