using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext context;
        private readonly ApplicationSettings settings;

        public UserService(ApplicationContext context, IConfiguration config)
        {
            this.context = context;
            settings = new ApplicationSettings(config);
        }

        public List<User> GetAll()
            => context.Users.ToList();

        public User? GetByEmail(string email)
            => context.Users.FirstOrDefault(user => user.Email == email);

        public User? GetByEmailAndPassword(string email, string password)
        {
            User? user = GetByEmail(email);

            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                return user;

            return null;
        }

        public bool IsUserAlreadyRegisteredWithEmail(string email)
            => GetByEmail(email) != null;

        public string GetHashedPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

        public void Add(User user)
        {
            user.Password = GetHashedPassword(user.Password);
            context.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Update(user);
            context.SaveChanges();
        }

        public RefreshTokenModel GenerateNewRefreshToken()
            => new RefreshTokenModel(Guid.NewGuid().ToString(), DateTime.UtcNow,
                                     DateTime.UtcNow.AddDays(settings.RefreshTokenExpirationDays));

        public void UpdateUserRefreshToken(User user)
        {
            var newRefreshToken = GenerateNewRefreshToken();

            user.RefreshToken = newRefreshToken.RefreshToken;
            user.RefreshTokenCreated = newRefreshToken.RefreshTokenCreated;
            user.RefreshTokenExpires = newRefreshToken.RefreshTokenExpires;

            Update(user);
        }
    }
}
