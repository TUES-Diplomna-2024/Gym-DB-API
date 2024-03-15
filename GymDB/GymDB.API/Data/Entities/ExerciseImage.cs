using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Entities
{
    public class ExerciseImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(Exercise))]
        public Guid ExerciseId { get; set; }

        public Exercise Exercise { get; set; }

        [StringLength(50, ErrorMessage = "FileExtension must be up to 50 characters long!")]
        public string FileExtension { get; set; }

        public int Position { get; set; }
    }
}
