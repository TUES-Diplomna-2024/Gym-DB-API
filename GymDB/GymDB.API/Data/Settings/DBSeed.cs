using GymDB.API.Data.Settings.DBSeedModels;
using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class DBSeed
    {
        private string defaultRole = "";

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

            DefaultRole = config.GetSection("DBSeed")["DefaultRole"]!;

            rootAdminSection.Bind(RootAdmin);

            if (RootAdmin.Username.IsNullOrEmpty() || RootAdmin.Email.IsNullOrEmpty() || RootAdmin.Password.IsNullOrEmpty())
                throw new InvalidOperationException("'DBSeed:RootAdmin' doesn't have all the necessary settings - 'Username', 'Email', and 'Password'!");
        }

        public Dictionary<string, string> Roles { get; private set; } = new Dictionary<string, string>();

        public string DefaultRole
        {
            get { return defaultRole; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'DBSeed:DefaultRole' could not be found or is empty!");

                if (!Roles.ContainsKey(value))
                    throw new InvalidOperationException($"Invalid 'DBSeed:DefaultRole'! Role '{value}' could not be found in 'DBSeed:Roles'!");

                value = value.ToUpper().Replace(" ", "_");

                if (value == "SUPER_ADMIN")
                    throw new InvalidOperationException("Invalid 'DBSeed:DefaultRole'! The default role cannot be set to 'Super Admin'! This role can be assigned only to the root admin!");

                defaultRole = value;
            }
        }

        public RootAdmin RootAdmin { get; private set; } = new RootAdmin();
    }
}
