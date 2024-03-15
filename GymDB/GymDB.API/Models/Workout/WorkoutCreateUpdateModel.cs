using System.ComponentModel.DataAnnotations;

namespace GymDB.API.Models.Workout
{
    public class WorkoutCreateUpdateModel
    {
        [StringLength(70, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 70 characters long!")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "Description must be up to 250 characters long!")]
        public string? Description { get; set; }

        public List<Guid>? Exercises { get; set; }
    }
}
