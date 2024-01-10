using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings.DBSeedModels
{
    public class RootUser
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
