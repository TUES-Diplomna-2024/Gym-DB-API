using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings;
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

        public string GenerateNewJwtToken(User user)
        {
            var expiration = DateTime.UtcNow.Add(settings.JwtSettings.TokenLifetime);

            var token = new JwtSecurityToken(
                issuer: settings.JwtSettings.Issuer,
                audience: settings.JwtSettings.Audience,
                claims: CreateClaims(user),
                expires: expiration,
                signingCredentials: CreateSigningCredentials()
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        public List<Claim> CreateClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString()),
                new Claim("role", user.Role.NormalizedName)
            };

            return claims;
        }

        public SigningCredentials CreateSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSettings.ServerSecretKey));

            return new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
