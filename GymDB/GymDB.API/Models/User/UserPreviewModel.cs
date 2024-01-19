using GymDB.API.Data.ValidationAttributes;
using UserClass = GymDB.API.Data.Entities.User;

namespace GymDB.API.Models.User
{
    public class UserPreviewModel
    {
        public UserPreviewModel(UserClass user)
        {
            Id          = user.Id;
            Username    = user.Username;
            RoleName    = user.Role.Name;
            RoleColor   = user.Role.Color;
            OnCreated   = user.OnCreated;
            Age         = CalculateAge(user.BirthDate);
        }

        public Guid Id { get; set; }

        public string Username { get; set; }

        public int Age { get; set; }

        public string RoleName { get; set; }

        public string RoleColor { get; set; }

        public DateOnly OnCreated { get; set; }

        private int CalculateAge(DateOnly birthDate)
        {
            DateOnly currDate = DateOnly.FromDateTime(DateTime.UtcNow);

            int age = currDate.Year - birthDate.Year;

            if (currDate.DayOfYear < birthDate.DayOfYear)
                age--;

            return age;
        }
    }
}
