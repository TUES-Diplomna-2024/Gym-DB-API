using GymDB.API.Data.Enums;

namespace GymDB.API.Models.User
{
    public class UserProfileModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public string RoleColor { get; set; }

        public Gender Gender { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public DateOnly BirthDate { get; set; }

        public DateOnly OnCreated { get; set; }
    }
}
