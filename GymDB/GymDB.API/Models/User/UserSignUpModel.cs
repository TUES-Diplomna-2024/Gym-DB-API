using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.User
{
    public class UserSignUpModel
    {
        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 32 characters!")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [BirthDate(ErrorMessage = "Invalid birth date!")]
        public DateOnly BirthDate { get; set; }

        [Password(ErrorMessage = "Invalid password!")]
        public string Password { get; set; }
    }
}
