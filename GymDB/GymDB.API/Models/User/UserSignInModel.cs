using GymDB.API.Attributes.DataValidation;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.User
{
    public class UserSignInModel
    {
        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [StringLength(256, ErrorMessage = "Email must be up to 256 characters long!")]
        public string Email { get; set; }

        [Password]
        public string Password { get; set; }
    }
}
