using GymDB.API.Data.Enums;

namespace GymDB.API.Models.User
{
    public class UserPreviewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string RoleColor { get; set; }

        public DateOnly OnCreated { get; set; }

        public AssignableRole? AssignableRole { get; set; }
    }
}
