using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GymDB.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationContext context;

        public UserRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await context.Users
                                .Include(user => user.Role)
                                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User?> GetCurrUserAsync(HttpContext context)
        {
            Guid id;

            if (Guid.TryParse(context.User.FindFirstValue("userId"), out id))
            {
                return await GetUserByIdAsync(id);
            }

            return null;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
                                .Include(user => user.Role)
                                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await context.Users
                                .Include(user => user.Role)
                                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            user.Password = GetHashedPassword(user.Password);
            user.Gender = user.Gender.ToLower();

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            user.OnModified = DateTime.UtcNow;

            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(User user)
        {
            // TODO - Remove all user related data
            /*RemoveUserRelatedData(user);*/

            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }

        private string GetHashedPassword(string password)
            => BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
    }
}
