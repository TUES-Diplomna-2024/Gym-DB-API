using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GymDB.API.Models.Workout;

namespace GymDB.API.Data.Entities
{
    public class Workout
    {
        public Workout() { }

        public Workout(WorkoutCreateModel input, User owner)
        {
            Id            = Guid.NewGuid();
            OnCreated     = DateOnly.FromDateTime(DateTime.UtcNow);
            OnModified    = DateTime.UtcNow;
            ExerciseCount = 0;

            Name          = input.Name;
            Description   = input.Description;
            UserId        = owner.Id;
            User          = owner;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(130, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 130 characters long!")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be up to 500 characters long!")]
        public string? Description { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public User User { get; set; }

        public int ExerciseCount { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
