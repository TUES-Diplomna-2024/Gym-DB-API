using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
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

        public User? GetByEmail(string email)
            => context.Users.FirstOrDefault(user => user.Email == email);

        public User? GetByEmailAndPassword(string email, string password)
            => context.Users.FirstOrDefault(user => user.Email == email && user.Password == password);

        public bool IsUserAlreadyRegisteredWithEmail(string email)
            => GetByEmail(email) != null;

        public void Add(User user)
        {
            context.Add(user);
            context.SaveChanges();
        }
    }
}
