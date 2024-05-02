using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Settings
{
    public class ConnectionStrings
    {
        [Required]
        public string PostgresConnection { get; init; }
    }
}
