using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasswordAttribute : ValidationAttribute
    {
        private static readonly string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{8,16}$";
        
        private readonly Regex re = new Regex(pattern);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? password = value as string;

            if (password == null || password == "")
                return new ValidationResult("Password cannot be null or empty!");

            if (!re.IsMatch(password))
                return new ValidationResult("Password must meet the following requirements: must have at least one lowercase letter, one uppercase letter, one digit, one special character (!@#$%^&*()_+), and length between 8 and 16 characters!");

            return ValidationResult.Success;
        }
    }
}
