using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUserService userService;
        private readonly ApplicationSettings settings;

        public RefreshTokenService(IUserService userService, IConfiguration config)
        {
            this.userService = userService;
            settings = new ApplicationSettings(config);
        }

        public RefreshTokenModel GenerateNewRefreshToken()
            => new RefreshTokenModel(Guid.NewGuid().ToString(), DateTime.UtcNow,
                                     DateTime.UtcNow.Add(settings.RefreshTokenLifetime));
    }
}
