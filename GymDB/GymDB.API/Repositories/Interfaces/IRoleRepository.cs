using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName);

        Task AddRoleAsync(Role role);

        Task AddRolesAsync(List<Role> roles);
    }
}
