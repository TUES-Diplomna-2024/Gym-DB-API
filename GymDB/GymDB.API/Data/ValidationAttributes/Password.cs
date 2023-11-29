using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Password : ValidationAttribute
    {
        static private readonly string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{8,16}$";
        private readonly Regex re = new Regex(pattern);

        public override bool IsValid(object? value)
            => value != null && re.IsMatch((string)value);
    }
}
