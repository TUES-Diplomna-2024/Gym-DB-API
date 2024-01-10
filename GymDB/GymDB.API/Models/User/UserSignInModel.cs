using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.User
{
    public class UserSignInModel
    {
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [StringLength(256, ErrorMessage = "Email must be up to 256 characters long!")]
        public string Email { get; set; }

        [Password(ErrorMessage = "Invalid password!")]
        public string Password { get; set; }
    }
}
