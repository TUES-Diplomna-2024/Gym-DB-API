using GymDB.API.Data.Settings.DBSeedModels;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class DBSeed
    {
        public DBSeed(IConfiguration config)
        {

            if (!config.GetSection("DBSeed").Exists())
                throw new InvalidOperationException("'DBSeed' section could not be found or is empty!");

            IConfigurationSection roleSection = config.GetSection("DBSeed:Roles");
            IConfigurationSection rootAdminSection = config.GetSection("DBSeed:RootAdmin");

            if (!roleSection.Exists())
                throw new InvalidOperationException("'DBSeed:Roles' section could not be found or is empty!");

            if (!rootAdminSection.Exists())
                throw new InvalidOperationException("'DBSeed:RootAdmin' section could not be found or is empty!");

            roleSection.Bind(Roles);

            foreach(KeyValuePair<string, string> pair in Roles)
            {
                if (pair.Key.IsNullOrEmpty() || pair.Value.IsNullOrEmpty())
                    throw new InvalidOperationException("There is an invalid role in 'DBSeed:Roles'!");
            }
            
            if (!Roles.ContainsKey("Super Admin"))
                throw new InvalidOperationException("Role 'Super Admin' could not be found in 'DBSeed:Roles'!");

            DefaultRole = config.GetSection("DBSeed")["DefaultRole"] ??
                          throw new InvalidOperationException("'DBSeed:DefaultRole' could not be found!");

            if (!Roles.ContainsKey(DefaultRole))
                throw new InvalidOperationException($"Invalid 'DBSeed:DefaultRole'! Role '{DefaultRole}' could not be found in 'DBSeed:Roles'!'");

            DefaultRole = DefaultRole.ToUpper().Replace(" ", "_");

            if (DefaultRole == "SUPER_ADMIN")
                throw new InvalidOperationException("Invalid 'DBSeed:DefaultRole'! The default role cannot be set to 'Super Admin'! This role can be assigned only to the root admin!");

            rootAdminSection.Bind(RootAdmin);

            if (RootAdmin.Username.IsNullOrEmpty() || RootAdmin.Email.IsNullOrEmpty() || RootAdmin.Password.IsNullOrEmpty())
                throw new InvalidOperationException("'DBSeed:RootAdmin' doesn't have all the necessary settings - 'Username', 'Email', and 'Password'!");
        }

        public Dictionary<string, string> Roles { get; private set; } = new Dictionary<string, string>();

        public string DefaultRole { get; private set; }

        public RootAdmin RootAdmin { get; private set; } = new RootAdmin();
    }
}
