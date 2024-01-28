using Microsoft.IdentityModel.Tokens;

namespace GymDB.API.Data.Settings
{
    public class JwtSettings
    {
        private string issuer = "";
        private string audience = "";
        private string serverSecretKey = "";

        public JwtSettings(IConfiguration config)
        {
            IConfigurationSection jwtSettings = config.GetSection("JwtSettings");

            if (!jwtSettings.Exists())
                throw new InvalidOperationException("'JwtSettings' section could not be found or is empty!");

            Issuer = jwtSettings["Issuer"]!;
            
            Audience = jwtSettings["Audience"]!;

            ServerSecretKey = jwtSettings["ServerSecretKey"]!;

            TimeSpan result;

            if (!TimeSpan.TryParse(jwtSettings["TokenLifetime"], out result))
                throw new InvalidOperationException("'JwtSettings:TokenLifetime' could not be found, is empty or is in invalid format!");

            TokenLifetime = result;
        }

        public string Issuer {
            get { return issuer; }
            private set {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'JwtSettings:Issuer' could not be found or is empty!");

                issuer = value;
            }
        }

        public string Audience {
            get { return audience; }
            private set {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'JwtSettings:Audience' could not be found or is empty!");

                audience = value;
            }
        }

        public string ServerSecretKey {
            get { return serverSecretKey; }
            private set {
                if (value.IsNullOrEmpty())
                    throw new InvalidOperationException("'JwtSettings:ServerSecretKey' could not be found or is empty!");

                serverSecretKey = value;
            }
        }

        public TimeSpan TokenLifetime { get; private set; }
    }
}
