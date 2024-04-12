using GymDB.API.Data;
using GymDB.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GymDB.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationSettings settings;

        public JwtService(IConfiguration config)
        {
            settings = new ApplicationSettings(config);
        }

        public string GenerateNewAccessToken(Guid userId, string roleNormalizedName)
        {
            var expiration = DateTime.UtcNow.Add(settings.JwtSettings.AccessTokenLifetime);

            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString()),
                new Claim("userRole", roleNormalizedName)
            };

            return GenerateNewJwtToken(expiration, claims);
        }

        public string GenerateNewRefreshToken(Guid userId)
        {
            var expiration = DateTime.UtcNow.Add(settings.JwtSettings.RefreshTokenLifetime);

            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            return GenerateNewJwtToken(expiration, claims);
        }

        private string GenerateNewJwtToken(DateTime expiration, Claim[] claims)
        {
            var token = new JwtSecurityToken(
                issuer: settings.JwtSettings.Issuer,
                audience: settings.JwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: CreateSigningCredentials()
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.ServerSecretKey));

            return new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
