using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings.DBSeedModels;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Models.User;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationSettings settings;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

        public RoleService(IConfiguration config, IRoleRepository roleRepository, IUserRepository userRepository)
        {
            settings = new ApplicationSettings(config);
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
        }

        public async Task EnsureRolesCreatedAsync()
        {
            Dictionary<string, string> rolesToBeAdded = new Dictionary<string, string>();

            foreach(KeyValuePair<string, string> pair in settings.DBSeed.Roles)
            {
                Role? role = await roleRepository.GetRoleByNormalizedNameAsync(GetRoleNameNormalized(pair.Key));

                if (role == null)
                {
                    rolesToBeAdded[pair.Key] = pair.Value;
                }
            }

            var roles = rolesToBeAdded.Select(pair => new Role
            {
                Id = Guid.NewGuid(),
                Name = pair.Key,
                NormalizedName = GetRoleNameNormalized(pair.Key),
                Color = pair.Value
            }).ToList();

            if (roles.Count != 0)
            {
                await roleRepository.AddRolesAsync(roles);
            }
        }

        public async Task EnsureRootAdminCreatedAsync()
        {
            User? rootAdmin = await userRepository.GetUserByEmailAsync(settings.DBSeed.RootAdmin.Email);

            if (rootAdmin == null)
            {
                rootAdmin = settings.DBSeed.RootAdmin.ToEntity();
                await userRepository.AddUserAsync(rootAdmin);
            }

            Role? superAdminRole = await roleRepository.GetRoleByNormalizedNameAsync("SUPER_ADMIN");

            if (superAdminRole == null)
            {
                superAdminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Super Admin",
                    NormalizedName = "SUPER_ADMIN",
                    Color = settings.DBSeed.Roles["Super Admin"]
                };

                await roleRepository.AddRoleAsync(superAdminRole);
            }

            if (rootAdmin.RoleId != superAdminRole.Id)
                await roleRepository.AddUserToRoleAsync(rootAdmin, superAdminRole);
        }

        public async Task AssignUserRoleAsync(HttpContext context, Guid userId, UserAssignRoleModel assignRoleModel)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"The specified user could not be found!");

            if (HasUserRole(user, assignRoleModel.RoleNormalizedName))
                throw new ForbiddenException("The specified user already has this role assigned!");

            if (assignRoleModel.RoleNormalizedName == "SUPER_ADMIN")
                throw new ForbiddenException("The specified role is reserved for the root admin only!");

            if (HasUserRole(user, "SUPER_ADMIN"))
                throw new ForbiddenException("The role of the root admin cannot be changed!");

            if (HasUserRole(user, "ADMIN") && !HasUserRole(currUser, "SUPER_ADMIN"))
                throw new ForbiddenException("You cannot re-assign new role to another admin user! This can be done only by the root admin!");

            if (!await roleRepository.AddUserToRoleAsync(user, assignRoleModel.RoleNormalizedName))
                throw new NotFoundException($"The specified role could not be found!");
        }

        public async Task AssignUserDefaultRoleAsync(User user)
        {
            string defaultRoleName = settings.DBSeed.DefaultRole;
            string defaultRoleNormalizedName = GetRoleNameNormalized(defaultRoleName);

            Role? defaultRole = await roleRepository.GetRoleByNormalizedNameAsync(defaultRoleNormalizedName);

            if (defaultRole == null)
            {
                defaultRole = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = defaultRoleName,
                    NormalizedName = defaultRoleNormalizedName,
                    Color = settings.DBSeed.Roles[defaultRoleName]
                };

                await roleRepository.AddRoleAsync(defaultRole);
            }

            await roleRepository.AddUserToRoleAsync(user, defaultRole);
        }

        public bool HasUserRole(User user, string role)
            => user.Role.NormalizedName == role;

        public bool HasUserAnyRole(User user, string[] roles)
            => roles.Contains(user.Role.NormalizedName);

        private string GetRoleNameNormalized(string roleName)
            => roleName.ToUpper().Replace(" ", "_");
    }
}
