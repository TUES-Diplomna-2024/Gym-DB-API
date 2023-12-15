namespace GymDB.API.Data
{
    public class ApplicationSettings
    {
        public ApplicationSettings(IConfiguration config)
        {
            PostgresConnectionString = config.GetConnectionString("PostgresConnection") ??
                                       throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

            IConfigurationSection jwtSettings = config.GetSection("JwtSettings");

            JwtIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found!");
            JwtAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not found!");
            JwtSecretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not found!");
            JwtExpirationMinutes = double.Parse(jwtSettings["ExpirationMinutes"] ??
                                                throw new InvalidOperationException("JWT ExpirationMinutes not found!"));
        }

        public string PostgresConnectionString { get; private set; }

        public string JwtIssuer { get; private set; }

        public string JwtAudience { get; private set; }

        public string JwtSecretKey { get; private set; }

        public double JwtExpirationMinutes { get; private set; }
    }
}
