using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Services.Interfaces;

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
    }
}
