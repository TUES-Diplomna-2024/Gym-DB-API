using WorkoutClass = GymDB.API.Data.Entities.Workout;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Models.Workout
{
    public class WorkoutWithExercisesModel
    {
        public WorkoutWithExercisesModel(WorkoutClass workout, List<ExercisePreviewModel> exercises)
        {
            Id             = workout.Id;
            Name           = workout.Name;
            Description    = workout.Description;
            ExerciseCount  = workout.ExerciseCount;
            this.exercises = exercises;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int ExerciseCount { get; set; }

        public List<ExercisePreviewModel> exercises { get; set; }
    }
}
