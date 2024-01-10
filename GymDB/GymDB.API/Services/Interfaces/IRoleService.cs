using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IRoleService
    {
        Role? GetByNormalizedName(string normalizedName);

        bool AssignUserRole(User user, string roleName);
    }
}
