using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GymDB.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateNewAccessToken(Guid userId, string roleNormalizedName);

        string GenerateNewRefreshToken(Guid userId);

        string GenerateNewJwtToken(DateTime expiration, Claim[] claims);

        SigningCredentials CreateSigningCredentials();
    }
}
