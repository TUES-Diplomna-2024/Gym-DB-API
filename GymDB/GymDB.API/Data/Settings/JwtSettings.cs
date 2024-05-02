using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GymDB.API.Data.Settings
{
    public class JwtSettings
    {
        [Required]
        public string Issuer { get; init; }

        [Required]
        public string Audience { get; init; }

        [Required]
        public string ServerSecretKey { get; init; }

        [Required]
        public TimeSpan AccessTokenLifetime { get; init; }

        [Required]
        public TimeSpan RefreshTokenLifetime { get; init; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
            => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ServerSecretKey));
    }
}
