using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings
{
    public class RootAdmin
    {
        [Required, StringLength(32, MinimumLength = 3)]
        public string Username { get; init; }

        [Required, StringLength(256), EmailAddress]
        public string Email { get; init; }

        [Required, Password]
        public string Password { get; init; }
    }
}
