using GymDB.API.Data.Entities;
using GymDB.API.Models;

namespace GymDB.API.Services.Interfaces
{
    public interface ISessionService
    {
        string CreateNewSession(User user);

        Session? GetSessionByRefreshToken(string refreshToken);

        List<Session> GetAllInactiveSessions();

        void Remove(Session session);

        void RemoveAllInactiveSessions();
    }
}
