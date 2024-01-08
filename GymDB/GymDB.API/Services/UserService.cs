using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Services.Interfaces;

namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext context;

        public UserService(ApplicationContext context)
        {
            this.context = context;
        }

        public bool AreAnyUsersRegistered()
            => context.Users.Any();

        public List<User> GetAll()
            => context.Users.ToList();

        public User? GetById(Guid id)
            => context.Users.FirstOrDefault(user => user.Id == id);

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
    }
}
