using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class ConnectionStrings
    {
        private string postgresConnection = "";

        public ConnectionStrings(IConfiguration config)
        {
            PostgresConnection = config.GetConnectionString("PostgresConnection")!;
        }

        public string PostgresConnection {
            get { return postgresConnection; }
            private set
            {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'ConnectionStrings:PostgresConnection' could not be found or is empty!");

                postgresConnection = value;
            }
        }
    }
}
