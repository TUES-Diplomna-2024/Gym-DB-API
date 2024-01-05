using GymDB.API.Data.Entities;
using GymDB.API.Models;

namespace GymDB.API.Services.Interfaces
{
    public interface ISessionService
    {
        string CreateNewSession(User user);

        Session? GetUserSessionByRefreshToken(Guid userId, string refreshToken);
    }
}
