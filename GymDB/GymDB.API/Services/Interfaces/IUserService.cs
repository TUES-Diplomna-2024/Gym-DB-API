using GymDB.API.Data.Entities;
using GymDB.API.Models.User;

namespace GymDB.API.Services.Interfaces
{
    public interface IUserService
    {
        User? GetByEmail(string email);

        User? GetByEmailAndPassword(string email, string password);

        bool IsUserAlreadyRegisteredWithEmail(string email);

        void Add(User user);
    }
}
