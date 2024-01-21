using WorkoutClass = GymDB.API.Data.Entities.Workout;

namespace GymDB.API.Models.Workout
{
    public class WorkoutPreviewModel
    {
        public WorkoutPreviewModel(WorkoutClass workout)
        {
            Id            = workout.Id;
            Name          = workout.Name;
            ExerciseCount = workout.ExerciseCount;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ExerciseCount { get; set; }
    }
}
