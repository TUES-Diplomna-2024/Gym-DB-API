using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GymDB.API.Data.Settings;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings jwtSettings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            jwtSettings = settings.Value;
        }

        public string GenerateNewAccessToken(Guid userId, string roleNormalizedName)
        {
            var expiration = DateTime.UtcNow.Add(jwtSettings.AccessTokenLifetime);

            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString()),
                new Claim("userRole", roleNormalizedName)
            };

            return GenerateNewJwtToken(expiration, claims);
        }

        public string GenerateNewRefreshToken(Guid userId)
        {
            var expiration = DateTime.UtcNow.Add(jwtSettings.RefreshTokenLifetime);

            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            return GenerateNewJwtToken(expiration, claims);
        }

        private string GenerateNewJwtToken(DateTime expiration, Claim[] claims)
        {
            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(jwtSettings.GetSymmetricSecurityKey(),
                                                           SecurityAlgorithms.HmacSha256)
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
