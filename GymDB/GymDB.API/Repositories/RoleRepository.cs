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

        public async Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName)
        {
            return await context.Roles
                                .FirstOrDefaultAsync(role => role.NormalizedName == normalizedName);
        }

        public async Task AddRoleAsync(Role role)
        {
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }

        public async Task AddRolesAsync(List<Role> roles)
        {
            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();
        }
    }
}
