using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IRoleService
    {
        Role? GetRoleByNormalizedName(string normalizedName);

        bool HasUserAnyRole(User user, string[] roles);

        bool AssignUserRole(User user, string roleNormalizedName);
    }
}
