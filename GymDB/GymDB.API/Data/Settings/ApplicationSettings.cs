namespace GymDB.API.Data.Settings
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration config)
        {
            ConnectionStrings = new ConnectionStrings(config);

            JwtSettings = new JwtSettings(config);

            SessionSettings = new SessionSettings(config);

            DBSeed = new DBSeed(config);
        }

        public ConnectionStrings ConnectionStrings { get; private set; }

        public JwtSettings JwtSettings { get; private set; }

        public SessionSettings SessionSettings { get; private set; }

        public DBSeed DBSeed { get; private set; }
    }
}
