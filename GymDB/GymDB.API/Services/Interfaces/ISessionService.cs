using GymDB.API.Data.Entities;
using GymDB.API.Models;

namespace GymDB.API.Services.Interfaces
{
    public interface ISessionService
    {
        string CreateNewSession(User user);

        Session? GetSessionByRefreshToken(string refreshToken);

        List<Session> GetAllUserSessions(User user);

        List<Session> GetAllInactiveSessions();

        void RemoveSession(Session session);

        void RemoveAllUserSessions(User user);

        void RemoveAllInactiveSessions();
    }
}
