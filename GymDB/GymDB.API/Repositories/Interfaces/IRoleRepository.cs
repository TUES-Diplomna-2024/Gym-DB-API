using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByNormalizedName(string normalizedName);

        Task<bool> IsRoleAssignedToAnyUser(Role role);

        // TODO: Add HasAnyRoles method

        Task AddRoles(List<Role> roles);
    }
}
