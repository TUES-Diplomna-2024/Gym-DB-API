using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.Workout
{
    public class WorkoutCreateModel
    {
        [StringLength(130, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 130 characters long!")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description must be up to 500 characters long!")]
        public string? Description { get; set; }

        public List<Guid>? Exercises { get; set; }
    }
}
