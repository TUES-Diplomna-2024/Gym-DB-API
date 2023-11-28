using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BirthDate : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            DateTime currDate = DateTime.Now;
            DateTime date;
            bool isParsed = DateTime.TryParse((string)value, out date);

            if (!isParsed || date.Year < currDate.Year - 122 || date > currDate)
                return false;

            return true;
        }
    }
}
