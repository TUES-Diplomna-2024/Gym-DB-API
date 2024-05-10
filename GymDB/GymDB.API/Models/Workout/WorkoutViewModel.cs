using GymDB.API.Models.Exercise;

namespace GymDB.API.Models.Workout
{
    public class WorkoutViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<ExercisePreviewModel>? Exercises { get; set; }
    }
}
