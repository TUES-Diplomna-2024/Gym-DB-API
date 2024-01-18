using GymDB.API.Data.ValidationAttributes;
using GymDB.API.Models.Exercise;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymDB.API.Data.Entities
{
    public class Exercise
    {
        public Exercise() { }

        public Exercise(ExerciseCreateModel input, User? user)
        {
            Id           = Guid.NewGuid();
            OnCreated    = DateOnly.FromDateTime(DateTime.UtcNow);
            OnModified   = DateTime.UtcNow;

            Name         = input.Name;
            Instructions = input.Instructions;
            MuscleGroups = input.MuscleGroups;
            Type         = input.Type;
            Difficulty   = input.Difficulty;
            Equipment    = input.Equipment;

            IsCustom     = input.UserId != null ? true : false;
            UserId       = input.UserId;
            User         = user;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(130, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 130 characters long!")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Instructions must be up to 500 characters long!")]
        public string Instructions { get; set; }

        public string MuscleGroups { get; set; }

        [ExerciseType]
        [StringLength(30)]
        public string Type { get; set; }

        [ExerciseDifficulty]
        [StringLength(12)]
        public string Difficulty { get; set; }

        public string? Equipment { get; set; }

        public bool IsCustom { get; set; }

        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }

        public User? User { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
