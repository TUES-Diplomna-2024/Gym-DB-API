using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GymDB.API.Attributes.DataValidation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HexColorAttribute : ValidationAttribute
    {
        private static readonly string pattern = @"^#(?:[0-9a-fA-F]{3}){1,2}$";

        private readonly Regex re = new Regex(pattern);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? hexColor = value as string;

            if (hexColor == null || hexColor == "")
                return new ValidationResult("Hex color cannot be null or empty!");

            if (!re.IsMatch(hexColor))
                return new ValidationResult("Invalid hexadecimal color code!");

            return ValidationResult.Success;
        }
    }
}
