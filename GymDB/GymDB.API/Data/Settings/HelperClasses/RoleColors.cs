using GymDB.API.Attributes.DataValidation;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings.HelperClasses
{
    public class RoleColors
    {
        [Required, HexColor]
        public string SuperAdmin { get; init; }

        [Required, HexColor]
        public string Admin { get; init; }

        [Required, HexColor]
        public string Normie { get; init; }
    }
}
