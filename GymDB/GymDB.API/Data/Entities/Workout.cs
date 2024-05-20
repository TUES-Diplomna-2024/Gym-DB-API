using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Data.Entities
{
    public class Workout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(70, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 70 characters long!")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Description must be up to 250 characters long!")]
        public string? Description { get; set; }

        [ForeignKey(nameof(User))]
        public Guid OwnerId { get; set; }

        public User Owner { get; set; }

        public int ExerciseCount { get; set; }

        public DateOnly OnCreated { get; set; }

        public DateTime OnModified { get; set; }
    }
}
