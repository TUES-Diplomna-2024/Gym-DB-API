using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ExerciseDifficultyAttribute : ValidationAttribute
    {
        private readonly string[] validDifficulties = { "beginner", "intermediate", "expert" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? type = value as string;

            if (type == null || type == "")
                return new ValidationResult("Type cannot be null or empty!");

            if (!validDifficulties.Contains(type.ToLower()))
                return new ValidationResult($"Invalid difficulty! Accepted values are: {string.Join(", ", validDifficulties.Select(d => $"'{d}'"))}!");

            return ValidationResult.Success;
        }
    }
}
