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
            Id          = Guid.NewGuid();
            Username    = input.Username;
            Email       = input.Email;
            Password    = input.Password;
            BirthDate   = input.BirthDate;
            OnCreated   = DateOnly.FromDateTime(DateTime.UtcNow);
            OnModified  = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(32, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 32 characters!")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address!")]
        public string Email { get; set; }

        [Password(ErrorMessage = "Invalid password!")]
        public string Password { get; set; }

        [BirthDate(ErrorMessage = "Invalid birth date!")]
        public DateOnly BirthDate { get; set; }

        [Gender(ErrorMessage = "Invalid gender!")]
        public string Gender { get; set; }

        [Range(minimum: 63, maximum: 251, ErrorMessage = "Height must be between 63 and 251 cm!")]
        public double Height { get; set; }

        [Range(minimum: 40, maximum: 140, ErrorMessage = "Weight must be between 40 and 140 kg!")]
        public double Weight { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
