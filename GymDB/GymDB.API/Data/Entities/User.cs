using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymDB.API.Data.ValidationAttributes;
using GymDB.API.Models.User;

namespace GymDB.API.Data.Entities
{
    public class User
    {
        public User() { }

        public User(UserSignUpModel input)
        {
            Id = Guid.NewGuid();
            Username = input.Username;
            Email = input.Email;
            BirthDate = input.BirthDate;
            Password = input.Password;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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
