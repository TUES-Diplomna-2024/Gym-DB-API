using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GymDB.API.Data;
using GymDB.API.Data.Entities;
using GymDB.API.Repositories.Interfaces;
using GymDB.API.Exceptions;

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

        public async Task<User> GetCurrUserAsync(HttpContext context)
        {
            if (Guid.TryParse(context.User.FindFirstValue("userId"), out Guid id))
            {
                User? currUser = await GetUserByIdAsync(id);

                if (currUser != null)
                {
                    return currUser;
                }
            }

            throw new UnauthorizedException("User is not currently signed in!");
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
                                .Include(user => user.Role)
                                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<List<User>> FindAllUsersMatchingUsernameOrEmailAsync(string query)
        {
            return await context.Users
                                .Include(user => user.Role)
                                .Where(user => user.Username.ToLower().Contains(query.ToLower()) ||
                                               user.Email.ToLower().Contains(query.ToLower()))
                                .OrderBy(user => user.Username)
                                .ThenBy(user => user.Email)
                                .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            user.Password = GetHashedPassword(user.Password);

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
