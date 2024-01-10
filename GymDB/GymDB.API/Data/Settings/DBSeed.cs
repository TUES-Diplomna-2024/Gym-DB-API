using GymDB.API.Data.Settings.DBSeedModels;

namespace GymDB.API.Data.Settings
{
    public class DBSeed
    {
        public DBSeed(IConfiguration config)
        {
            config.GetSection("DBSeed:Roles").Bind(Roles);

            foreach(KeyValuePair<string, Role> pair in Roles)
            {
                if (pair.Key == null || pair.Value.NormalizedName == null || pair.Value.Color == null)
                    throw new InvalidOperationException("There is an invalid role in 'DBSeed:Roles'!");
            }

            DefaultRole = config.GetSection("DBSeed")["DefaultRole"] ??
                          throw new InvalidOperationException("'DBSeed:DefaultRole' could not be found!");

            if (Roles.Values.FirstOrDefault(role => role.NormalizedName == DefaultRole) == null)
                throw new InvalidOperationException($"Invalid 'DBSeed:DefaultRole'! Role '{DefaultRole}' wasn't defined in 'DBSeed:Roles' or wasn't written in normalized form!'");

            config.GetSection("DBSeed:RootUser").Bind(RootUser);

            if (RootUser.Username == null || RootUser.Email == null || RootUser.Password == null || RootUser.Role == null)
                throw new InvalidOperationException("Some required settings such as 'Username', 'Email', 'Password' and 'RoleNormalizedName' weren't defined in 'DBSeed:RootUser'!");

            if (Roles.Values.FirstOrDefault(role => role.NormalizedName == RootUser.Role) == null)
                throw new InvalidOperationException($"Invalid 'DBSeed:RootUser:RoleNormalizedName'! Role '{RootUser.Role}' wasn't defined in 'DBSeed:Roles' or wasn't written in normalized form!");
        }

        public Dictionary<string, Role> Roles { get; private set; } = new Dictionary<string, Role>();

        public string DefaultRole { get; private set; }

        public RootUser RootUser { get; private set; } = new RootUser();
    }
}
