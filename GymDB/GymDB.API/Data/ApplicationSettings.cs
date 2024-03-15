using GymDB.API.Data.Settings;

namespace GymDB.API.Data
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration config)
        {
            ConnectionStrings = new ConnectionStrings(config);

            AzureSettings = new AzureSettings(config);

            JwtSettings = new JwtSettings(config);

            DBSeed = new DBSeed(config);
        }

        public ConnectionStrings ConnectionStrings { get; private set; }

        public AzureSettings AzureSettings { get; private set; }

        public JwtSettings JwtSettings { get; private set; }

        public DBSeed DBSeed { get; private set; }
    }
}
