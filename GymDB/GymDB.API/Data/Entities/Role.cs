using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDB.API.Data.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(60, ErrorMessage = "Name must be up to 60 characters long!")]
        public string Name { get; set; }

        [StringLength(60, ErrorMessage = "NormalizedName must be up to 60 characters long!")]
        public string NormalizedName { get; set; }

        [StringLength(256, ErrorMessage = "Color must be up to 256 characters long!")]
        public string Color { get; set; }
    }
}
