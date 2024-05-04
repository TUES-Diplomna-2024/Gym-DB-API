namespace GymDB.API.Models.User
{
    public class UserPreviewModel
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public int Age { get; set; }

        public string RoleName { get; set; }

        public string RoleColor { get; set; }

        public DateOnly OnCreated { get; set; }
    }
}
