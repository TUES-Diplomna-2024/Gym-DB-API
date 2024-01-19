using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Models.User;
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

        public List<UserPreviewModel> GetAllUserPreviews()
            => GetAllUsers().Select(user => new UserPreviewModel(user))
                            .OrderBy(user => user.RoleName)
                            .ThenBy(user => user.Username)
                            .ToList(); 

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

            if (user != null && IsUserPasswordCorrect(user, password))
                return user;

            return null;
        }

        public bool IsUserAlreadyRegisteredWithEmail(string email)
            => GetUserByEmail(email) != null;

        public bool IsUserPasswordCorrect(User user, string password)
            => BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);

        public string GetHashedPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);

        public void AddUser(User user)
        {
            user.Password = GetHashedPassword(user.Password);
            user.Gender = user.Gender.ToLower();

            context.Users.Add(user);
            context.SaveChanges();
        }

        public void UpdateUser(User user, UserUpdateModel update)
        {
            user.Username = update.Username;
            user.BirthDate = update.BirthDate;
            user.Gender = update.Gender;
            user.Height = update.Height;
            user.Weight = update.Weight;

            UpdateUser(user);
        }

        public void UpdateUser(User user)
        {
            user.OnModified = DateTime.UtcNow;

            context.Users.Update(user);
            context.SaveChanges();
        }

        public void RemoveUser(User user)
        {
            context.Users.Remove(user);
            context.SaveChanges();
        }
    }
}
