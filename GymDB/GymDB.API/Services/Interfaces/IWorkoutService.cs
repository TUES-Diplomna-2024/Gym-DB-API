using GymDB.API.Data.Entities;
using GymDB.API.Models.Workout;

namespace GymDB.API.Services.Interfaces
{
    public interface IWorkoutService
    {
        List<Workout> GetUserWorkouts(User user);

        List<WorkoutPreviewModel> GetWorkoutsPreviews(List<Workout> workouts);

        WorkoutWithExercisesModel GetWorkoutWithExercises(Workout workout);

        Workout? GetWorkoutById(Guid id);

        void AddWorkout(Workout workout);

        void AddExercisesToWorkout(Workout workout, List<Exercise> exercises);
    }
}
