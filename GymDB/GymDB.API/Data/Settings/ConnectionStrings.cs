namespace GymDB.API.Data.Settings
{
    public class ConnectionStrings
    {
        public ConnectionStrings(IConfiguration config)
        {
            PostgresConnection = config.GetConnectionString("PostgresConnection") ??
                                 throw new InvalidOperationException("'ConnectionStrings:PostgresConnection' could not be found!");
        }

        public string PostgresConnection { get; private set; }
    }
}
