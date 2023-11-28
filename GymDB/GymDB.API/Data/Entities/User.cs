using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Entities
{
    public class User
    {
        User() { }

        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 32 characters!")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [BirthDate(ErrorMessage = "Invalid birth date!")]
        public DateTime BirthDate { get; set; }

        [Password(ErrorMessage = "Invalid password!")]
        public string Password { get; set; }
    }
}
