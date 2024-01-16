using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymDB.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationContext context;

        public UserService(ApplicationContext context)
        {
            this.context = context;
        }

        public List<User> GetAllUsers()
            => context.Users.Include(user => user.Role).ToList();

        public User? GetUserById(Guid id)
            => context.Users.Include(user => user.Role)
                            .FirstOrDefault(user => user.Id == id);

        public User? GetCurrUser(HttpContext context)
        {
            Guid id;

            if (Guid.TryParse(context.User.FindFirstValue("userId"), out id))
                return GetUserById(id);

            return null;
        }

        public User? GetUserByEmail(string email)
            => context.Users.Include(user => user.Role)
                            .FirstOrDefault(user => user.Email == email);

        public User? GetUserByEmailAndPassword(string email, string password)
        {
            User? user = GetUserByEmail(email);

            if (user != null && BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password))
                return user;

            return null;
        }

        public bool IsUserAlreadyRegisteredWithEmail(string email)
            => GetUserByEmail(email) != null;

        public string GetHashedPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

        public void AddUser(User user)
        {
            user.Password = GetHashedPassword(user.Password);
            user.Gender = user.Gender.ToLower();

            context.Add(user);
            context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            context.Update(user);
            context.SaveChanges();
        }
    }
}
