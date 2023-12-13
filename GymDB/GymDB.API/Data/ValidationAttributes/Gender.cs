using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class Gender : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            string[] genders = { "male", "female", "other" };

            return genders.Contains(((string)value).ToLower());
        }
    }
}
