using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.User
{
    public class UserSignUpModel
    {
        [StringLength(32, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 32 characters!")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [StringLength(256, ErrorMessage = "Email must be up to 256 characters long!")]
        public string Email { get; set; }

        [Password(ErrorMessage = "Invalid password!")]
        public string Password { get; set; }

        [BirthDate(ErrorMessage = "Invalid birth date!")]
        public DateOnly BirthDate { get; set; }

        [Gender(ErrorMessage = "Invalid gender!")]
        [StringLength(10, ErrorMessage = "Gender must be up to 10 characters long!")]
        public string Gender { get; set; }

        [Range(minimum: 63, maximum: 251, ErrorMessage = "Height must be between 63 and 251 cm!")]
        public double Height { get; set; }

        [Range(minimum: 40, maximum: 140, ErrorMessage = "Weight must be between 40 and 140 kg!")]
        public double Weight { get; set; }
    }
}
