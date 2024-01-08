using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Migrations;
using GymDB.API.Models;
using GymDB.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Services
{
    public class SessionService : ISessionService
    {
        private readonly ApplicationContext context;
        private readonly ApplicationSettings settings;

        public SessionService(ApplicationContext context, IConfiguration config)
        {
            this.context = context;
            settings = new ApplicationSettings(config);
        }

        public string CreateNewSession(User user)
        {
            string refreshToken = Guid.NewGuid().ToString();
            
            Session session = new Session()
            {
                Id = Guid.NewGuid(),
                RefreshToken = refreshToken,
                ExpireDate = DateTime.UtcNow.Add(settings.SessionLifetime),
                UserId = user.Id,
                User = user
            };

            context.Sessions.Add(session);
            context.SaveChanges();

            return refreshToken;
        }

        public Session? GetById(Guid sessionId)
            => context.Sessions.Include(session => session.User)
                               .FirstOrDefault(session => session.Id == sessionId);

        public Session? GetUserSessionByRefreshToken(Guid userId, string refreshToken)
            => context.Sessions.Include(session => session.User)
                               .FirstOrDefault(session => session.UserId == userId && session.RefreshToken == refreshToken);

        public List<Session> GetAllInactiveSessions()
        {
            return context.Sessions.Include(session => session.User)
                                   .Where(session => session.ExpireDate < DateTime.UtcNow)
                                   .ToList();
        }

        public void Remove(Session session)
        {
            context.Sessions.Remove(session);
            context.SaveChanges();
        }

        public void RemoveAllInactiveSessions()
        {
            List<Session> toBeRemoved = GetAllInactiveSessions();
            context.Sessions.RemoveRange(toBeRemoved);
            context.SaveChanges();
        }
    }
}
