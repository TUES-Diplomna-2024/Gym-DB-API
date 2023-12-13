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
            Gender    = user.Gender;
            Height    = user.Height;
            Weight    = user.Weight;
            BirthDate = user.BirthDate;
            OnCreated = user.OnCreated;
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public DateOnly BirthDate { get; set; }

        public DateOnly OnCreated { get; set; }
    }
}
