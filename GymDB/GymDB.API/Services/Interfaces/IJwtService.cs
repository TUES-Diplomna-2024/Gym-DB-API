using GymDB.API.Data.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GymDB.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateNewJwtToken(User user);

        List<Claim> CreateClaims(User user);

        SigningCredentials CreateSigningCredentials();
    }
}
