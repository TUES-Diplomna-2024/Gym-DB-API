namespace GymDB.API.Data.Settings
{
    public class JwtSettings
    {
        public JwtSettings(IConfiguration config)
        {
            IConfigurationSection jwtSettings = config.GetSection("JwtSettings");

            if (!jwtSettings.Exists())
                throw new InvalidOperationException("'JwtSettings' section could not be found or is empty!");

            Issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("'JwtSettings:Issuer' could not be found!");
            
            Audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("'JwtSettings:Audience' could not be found!");
            
            ServerSecretKey = jwtSettings["ServerSecretKey"] ??
                              throw new InvalidOperationException("'JwtSettings:ServerSecretKey' could not be found!");
            
            TokenLifetime = TimeSpan.Parse(jwtSettings["TokenLifetime"] ??
                            throw new InvalidOperationException("'JwtSettings:TokenLifetime' could not be found!"));
        }

        public string Issuer { get; private set; }

        public string Audience { get; private set; }

        public string ServerSecretKey { get; private set; }

        public TimeSpan TokenLifetime { get; private set; }
    }
}
