using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GymDB.API.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationContext context;
        private readonly IUserRepository userRepository;

        public RoleRepository(ApplicationContext context, IUserRepository userRepository)
        {
            this.context = context;
            this.userRepository = userRepository;
        }

        public async Task<Role?> GetRoleByNormalizedNameAsync(string normalizedName)
        {
            return await context.Roles
                                .FirstOrDefaultAsync(role => role.NormalizedName == normalizedName);
        }

        public async Task UpdateUserRoleAsync(User user, string roleNormalizedName)
        {
            Role? role = await GetRoleByNormalizedNameAsync(roleNormalizedName);

            if (role == null)
                throw new NotFoundException("The specified role could not be found!");

            user.SetRole(role);
            await userRepository.UpdateUserAsync(user);
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
