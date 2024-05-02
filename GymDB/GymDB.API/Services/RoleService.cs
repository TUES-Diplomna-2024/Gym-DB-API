using GymDB.API.Data.Entities;
using GymDB.API.Data.Enums;
using GymDB.API.Data.Settings;
using GymDB.API.Data.Settings.HelperClasses;
using GymDB.API.Exceptions;
using GymDB.API.Mappers;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace GymDB.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleSettings roleSettings;
        private readonly RootAdmin rootAdminSettings;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

        public RoleService(IOptions<RoleSettings> roleSettings, IOptions<RootAdmin> rootAdminSettings, IRoleRepository roleRepository, IUserRepository userRepository)
        {
            this.roleSettings = roleSettings.Value;
            this.rootAdminSettings = rootAdminSettings.Value;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
        }

        public async Task EnsureRolesCreatedAsync()
        {
            List<RoleDefinition> rolesToBeAdded = new List<RoleDefinition>();

            foreach(var roleDef in roleSettings)
            {
                Role? role = await roleRepository.GetRoleByNormalizedNameAsync(roleDef.NormalizedName);

                if (role == null)
                    rolesToBeAdded.Add(roleDef);
            }

            var roles = rolesToBeAdded.Select(roleDef => roleDef.ToEntity()).ToList();

            if (roles.Count != 0)
                await roleRepository.AddRolesAsync(roles);
        }

        public async Task EnsureRootAdminCreatedAsync()
        {
            User? rootAdmin = await userRepository.GetUserByEmailAsync(rootAdminSettings.Email);

            if (rootAdmin != null)
                return;

            rootAdmin = rootAdminSettings.ToEntity();

            Role? superAdminRole = await roleRepository.GetRoleByNormalizedNameAsync(roleSettings.SuperAdmin.NormalizedName);

            if (superAdminRole == null)
            {
                superAdminRole = roleSettings.SuperAdmin.ToEntity();
                await roleRepository.AddRoleAsync(superAdminRole);
            }

            rootAdmin.SetRole(superAdminRole);
            await userRepository.AddUserAsync(rootAdmin);
        }

        public async Task AssignUserDefaultRoleAsync(User user)
        {
            Role? defaultRole = await roleRepository.GetRoleByNormalizedNameAsync(roleSettings.Normie.NormalizedName);

            if (defaultRole == null)
            {
                defaultRole = roleSettings.Normie.ToEntity();
                await roleRepository.AddRoleAsync(defaultRole);
            }

            user.SetRole(defaultRole);
        }

        public async Task AssignUserNewRoleAsync(HttpContext context, Guid userId, AssignableRole role)
        {
            User currUser = await userRepository.GetCurrUserAsync(context);

            User? user = await userRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new NotFoundException($"The specified user could not be found!");

            if (HasUserRole(user, role.ToString().ToUpper()))
                throw new ForbiddenException("The specified user already has this role assigned!");

            if (HasUserRole(user, "SUPER_ADMIN"))
                throw new ForbiddenException("The role of the root admin cannot be changed!");

            if (HasUserRole(user, "ADMIN") && !HasUserRole(currUser, "SUPER_ADMIN"))
                throw new ForbiddenException("You cannot re-assign new role to another admin user! This can be done only by the root admin!");

            await roleRepository.UpdateUserRoleAsync(user, role.ToString().ToUpper());
        }

        public bool HasUserRole(User user, string role)
            => user.Role.NormalizedName == role;

        // TODO: Should be removed if not used
        public bool HasUserAnyRole(User user, string[] roles)
            => roles.Contains(user.Role.NormalizedName);

        // TODO: Should be removed if not used
        private string GetRoleNameNormalized(string roleName)
            => roleName.ToUpper().Replace(" ", "_");
    }
}
