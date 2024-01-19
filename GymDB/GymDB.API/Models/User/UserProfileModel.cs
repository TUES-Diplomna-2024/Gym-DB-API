using UserClass = GymDB.API.Data.Entities.User;

namespace GymDB.API.Models.User
{
    public class UserProfileModel
    {
        public UserProfileModel(UserClass user)
        {
            Username  = user.Username;
            Email     = user.Email;
            RoleName  = user.Role.Name;
            RoleColor = user.Role.Color;
            Gender    = user.Gender;
            Height    = user.Height;
            Weight    = user.Weight;
            BirthDate = user.BirthDate;
            OnCreated = user.OnCreated;
        }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public string RoleColor { get; set; }

        public string Gender { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public DateOnly BirthDate { get; set; }

        public DateOnly OnCreated { get; set; }
    }
}
