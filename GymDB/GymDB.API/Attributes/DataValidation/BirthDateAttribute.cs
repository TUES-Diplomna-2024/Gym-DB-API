using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BirthDateAttribute : ValidationAttribute
    {
        private readonly int allowableYearsRange = 122;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Date cannot be null!");

            DateTime currDate = DateTime.Now;
            DateTime date;
            bool isParsed = DateTime.TryParse(value.ToString(), out date);

            if (!isParsed)
                return new ValidationResult("Invalid date format!");

            if (date.Year < currDate.Year - allowableYearsRange || date > currDate)
                return new ValidationResult($"Date must be within the last {allowableYearsRange} years and cannot be in the future!");

            return ValidationResult.Success;
        }
    }
}
