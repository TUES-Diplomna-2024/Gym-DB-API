using GymDB.API.Data.Entities;
using GymDB.API.Models;
using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        User? GetUserById(Guid id);

        User? GetCurrUser(HttpContext context);

        User? GetUserByEmail(string email);

        User? GetUserByEmailAndPassword(string email, string password);

        bool IsUserAlreadyRegisteredWithEmail(string email);

        bool IsUserPasswordCorrect(User user, string password);

        string GetHashedPassword(string password);

        void AddUser(User user);

        void UpdateUser(User user, UserUpdateModel update);

        void UpdateUser(User user);

        void RemoveUser(User user);
    }
}
