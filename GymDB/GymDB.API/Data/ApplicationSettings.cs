namespace GymDB.API.Data
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration config)
        {
            PostgresConnectionString = config.GetConnectionString("PostgresConnection") ??
                                       throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");
        }

        public string PostgresConnectionString { get; private set; }
    }
}
