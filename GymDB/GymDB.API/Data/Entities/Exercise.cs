using GymDB.API.Data.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDB.API.Data.Entities
{
    public class Exercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(70, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 70 characters long!")]
        public string Name { get; set; }

        [StringLength(250, MinimumLength = 1, ErrorMessage = "Instructions must be between 1 and 250 characters long!")]
        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        [ExerciseType]
        [StringLength(30)]
        public string Type { get; set; }

        [ExerciseDifficulty]
        [StringLength(12)]
        public string Difficulty { get; set; }

        public string? Equipment { get; set; }

        public int ImageCount { get; set; }

        public bool IsPrivate { get; set; }

        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
