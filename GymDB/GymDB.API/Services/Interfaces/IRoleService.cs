using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IRoleService
    {
        Role? GetRoleByNormalizedName(string normalizedName);

        /// <summary>
        /// Uses role's normalized name
        /// </summary>
        bool HasUserRole(User user, string role);

        /// <summary>
        /// Uses role's normalized name
        /// </summary>
        bool HasUserAnyRole(User user, string[] roles);

        /// <summary>
        /// Uses role's normalized name
        /// </summary>
        bool AssignUserRole(User user, string role);
    }
}
