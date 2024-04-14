using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationContext context;

        public RoleRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Role?> GetRoleByNormalizedName(string normalizedName)
        {
            return await context.Roles
                                .FirstOrDefaultAsync(role => role.NormalizedName == normalizedName);
        }

        public async Task AddRoles(List<Role> roles)
        {
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }
    }
}
