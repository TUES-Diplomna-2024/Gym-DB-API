using GymDB.API.Data.Settings.DBSeedModels;

namespace GymDB.API.Data.Settings
{
    public class DBSeed
    {
        public DBSeed(IConfiguration config)
        {
            config.GetSection("DBSeed:Roles").Bind(Roles);

            foreach(KeyValuePair<string, string> pair in Roles)
            {
                if (pair.Key == null || pair.Value == null)
                    throw new InvalidOperationException("There is an invalid role in 'DBSeed:Roles'!");
            }
            
            if (!Roles.ContainsKey("Super Admin"))
                throw new InvalidOperationException("Role 'Super Admin' could not be found in 'DBSeed:Roles'!");

            DefaultRole = config.GetSection("DBSeed")["DefaultRole"] ??
                          throw new InvalidOperationException("'DBSeed:DefaultRole' could not be found!");

            if (!Roles.ContainsKey(DefaultRole))
                throw new InvalidOperationException($"Invalid 'DBSeed:DefaultRole'! Role '{DefaultRole}' could not be found in 'DBSeed:Roles'!'");

            DefaultRole = DefaultRole.ToUpper().Replace(" ", "_");

            if (DefaultRole == "Super Admin")
                throw new InvalidOperationException("Invalid 'DBSeed:DefaultRole'! The default role can't be set to 'Super Admin'! This role can be assigned only to the root admin!");

            config.GetSection("DBSeed:RootAdmin").Bind(RootAdmin);

            if (RootAdmin.Username == null || RootAdmin.Email == null || RootAdmin.Password == null)
                throw new InvalidOperationException("'DBSeed:RootAdmin' doesn't have all the necessary settings - 'Username', 'Email', and 'Password'!");
        }

        public Dictionary<string, string> Roles { get; private set; } = new Dictionary<string, string>();

        public string DefaultRole { get; private set; }

        public RootAdmin RootAdmin { get; private set; } = new RootAdmin();
    }
}
