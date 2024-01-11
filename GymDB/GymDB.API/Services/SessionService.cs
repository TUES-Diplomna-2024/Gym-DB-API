using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Data.Settings;
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
                ExpireDate = DateTime.UtcNow.Add(settings.SessionSettings.SessionLifetime),
                UserId = user.Id,
                User = user
            };

            context.Sessions.Add(session);
            context.SaveChanges();

            return refreshToken;
        }

        public Session? GetSessionByRefreshToken(string refreshToken)
            => context.Sessions.Include(session => session.User.Role)
                               .FirstOrDefault(session => session.RefreshToken == refreshToken);

        public List<Session> GetAllInactiveSessions()
        {
            return context.Sessions.Include(session => session.User.Role)
                                   .Where(session => session.ExpireDate < DateTime.UtcNow)
                                   .ToList();
        }

        public void RemoveSession(Session session)
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
