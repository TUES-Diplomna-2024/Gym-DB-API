using GymDB.API.Data.Entities;
using GymDB.API.Models;

namespace GymDB.API.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        RefreshTokenModel GenerateNewRefreshToken();
    }
}
