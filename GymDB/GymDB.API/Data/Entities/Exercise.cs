using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GymDB.API.Data.Enums;

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

        public ExerciseType Type { get; set; }

        public ExerciseDifficulty Difficulty { get; set; }

        public string? Equipment { get; set; }

        public int ImageCount { get; set; }

        public ExerciseVisibility Visibility { get; set; }

        [ForeignKey(nameof(User))]
        public Guid? OwnerId { get; set; }

        public User? Owner { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
