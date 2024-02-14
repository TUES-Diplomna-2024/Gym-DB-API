using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExerciseType : ValidationAttribute
    {
        private readonly string[] validTypes = { "cardio", "weightlifting", "plyometrics", "powerlifting",
                                                 "strength", "stretching", "strongman", "other" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? type = value as string;

            if (type == null || type == "")
                return new ValidationResult("Type cannot be null or empty!");

            if (!validTypes.Contains(type.ToLower()))
                return new ValidationResult($"Invalid type! Accepted values are: {string.Join(", ", validTypes.Select(t => $"'{t}'"))}!");

            return ValidationResult.Success;
        }
    }
}
