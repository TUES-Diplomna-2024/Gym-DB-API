using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymDB.API.Data.ValidationAttributes;
using GymDB.API.Models;
using GymDB.API.Models.User;
using Microsoft.EntityFrameworkCore;

namespace GymDB.API.Data.Entities
{
    public class User
    {
        public User() { }

        public User(UserSignUpModel input)
        {
            Id                  = Guid.NewGuid();
            OnCreated           = DateOnly.FromDateTime(DateTime.UtcNow);
            OnModified          = DateTime.UtcNow;
            
            Username            = input.Username;
            Email               = input.Email;
            Password            = input.Password;
            BirthDate           = input.BirthDate;
            Gender              = input.Gender;
            Height              = input.Height;
            Weight              = input.Weight;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(32, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 32 characters long!")]
        public string Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address!")]
        [StringLength(256, ErrorMessage = "Email must be up to 256 characters long!")]
        public string Email { get; set; }

        public string Password { get; set; }

        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }

        public Role Role { get; set; }

        [BirthDate(ErrorMessage = "Invalid birth date!")]
        public DateOnly BirthDate { get; set; }

        [Gender(ErrorMessage = "Invalid gender!")]
        [StringLength(10, ErrorMessage = "Gender must be up to 10 characters long!")]
        public string Gender { get; set; }

        [Range(minimum: 63, maximum: 251, ErrorMessage = "Height must be between 63 and 251 cm!")]
        public double Height { get; set; }

        [Range(minimum: 40, maximum: 140, ErrorMessage = "Weight must be between 40 and 140 kg!")]
        public double Weight { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
