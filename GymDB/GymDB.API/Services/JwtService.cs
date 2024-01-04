using GymDB.API.Data;
using GymDB.API.Data.Entities;
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
            var expiration = DateTime.UtcNow.Add(settings.JwtLifetime);

            var token = new JwtSecurityToken(
                issuer: settings.JwtIssuer,
                audience: settings.JwtAudience,
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
                new Claim("userId", user.Id.ToString())
            };

            return claims;
        }

        public SigningCredentials CreateSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtSecretKey));

            return new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}
