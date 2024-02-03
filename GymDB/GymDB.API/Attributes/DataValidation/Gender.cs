using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Gender : ValidationAttribute
    {
        private readonly string[] validGenders = { "male", "female", "other" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? gender = value as string;

            if (gender == null || gender == "")
                return new ValidationResult("Gender cannot be null or empty!");

            if (!validGenders.Contains(gender.ToLower()))
                return new ValidationResult($"Invalid gender! Accepted values are: {string.Join(", ", validGenders.Select(g => $"'{g}'"))}!");
            
            return ValidationResult.Success;
        }
    }
}
