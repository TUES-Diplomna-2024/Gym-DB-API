using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName);

        Task<bool> AddUserToRoleAsync(User user, string roleNormalizedName);

        Task AddUserToRoleAsync(User user, Role role);

        Task AddRoleAsync(Role role);

        Task AddRolesAsync(List<Role> roles);
    }
}
