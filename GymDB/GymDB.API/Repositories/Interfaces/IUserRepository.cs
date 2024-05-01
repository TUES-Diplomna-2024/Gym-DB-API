using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdAsync(Guid id);

        Task<User> GetCurrUserAsync(HttpContext context);

        Task<User?> GetUserByEmailAsync(string email);

        Task<List<User>> GetAllUsersAsync();

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task RemoveUserAsync(User user);
    }
}
