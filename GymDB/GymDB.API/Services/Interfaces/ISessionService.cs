using GymDB.API.Data.Entities;
using GymDB.API.Models;

namespace GymDB.API.Services.Interfaces
{
    public interface ISessionService
    {
        string CreateNewSession(User user);

        Session? GetById(Guid sessionId);

        Session? GetUserSessionByRefreshToken(Guid userId, string refreshToken);

        List<Session> GetAllInactiveSessions();

        void Remove(Session session);

        void RemoveAllInactiveSessions();
    }
}
