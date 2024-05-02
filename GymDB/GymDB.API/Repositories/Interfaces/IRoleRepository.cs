using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName);

        Task UpdateUserRoleAsync(User user, string roleNormalizedName);

        Task AddRoleAsync(Role role);

        Task AddRolesAsync(List<Role> roles);
    }
}
