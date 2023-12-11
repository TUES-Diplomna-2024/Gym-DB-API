using UserClass = GymDB.API.Data.Entities.User;
using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.User
{
    public class UserProfileModel
    {
        public UserProfileModel(UserClass user)
        {
            Id        = user.Id;
            Username  = user.Username;
            Email     = user.Email;
            BirthDate = user.BirthDate;
            OnCreated = user.OnCreated;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateOnly BirthDate { get; set; }

        public DateOnly OnCreated { get; set; }
    }
}
