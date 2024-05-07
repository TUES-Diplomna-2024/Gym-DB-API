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

            Role superAdminRole = await GetOrCreateRoleAsync(roleSettings.SuperAdmin.NormalizedName);

            rootAdmin.SetRole(superAdminRole);
            await userRepository.AddUserAsync(rootAdmin);
        }

        public async Task AssignUserDefaultRoleAsync(User user)
        {
            Role defaultRole = await GetOrCreateRoleAsync(roleSettings.Normie.NormalizedName);
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

            if (IsUserSuperAdmin(user))
                throw new ForbiddenException("The role of the root admin cannot be changed!");

            if (IsUserAdmin(user) && !IsUserSuperAdmin(currUser))
                throw new ForbiddenException("You cannot re-assign new role to another admin user! This can be done only by the root admin!");

            Role roleToBeAssigned = await GetOrCreateRoleAsync(role.ToString().ToUpper());

            user.SetRole(roleToBeAssigned);
            await userRepository.UpdateUserAsync(user);
        }

        public bool IsUserNormie(User user)
            => HasUserRole(user, "NORMIE");

        public bool IsUserAdmin(User user)
            => HasUserRole(user, "ADMIN");

        public bool IsUserSuperAdmin(User user)
            => HasUserRole(user, "SUPER_ADMIN");

        private async Task<Role> GetOrCreateRoleAsync(string normalizedRoleName)
        {
            Role? role = await roleRepository.GetRoleByNormalizedNameAsync(normalizedRoleName);

            if (role != null)
                return role;

            foreach (RoleDefinition roleDef in roleSettings)
            {
                if (roleDef.NormalizedName == normalizedRoleName)
                {
                    Role roleToBeCreated = roleDef.ToEntity();
                    await roleRepository.AddRoleAsync(roleToBeCreated);

                    return roleToBeCreated;
                }
            }

            throw new NotFoundException("The specified role could not be found!");
        }

        private bool HasUserRole(User user, string role)
            => user.Role.NormalizedName == role;
    }
}
