using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings.DBSeedModels
{
    public class RootAdmin
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
