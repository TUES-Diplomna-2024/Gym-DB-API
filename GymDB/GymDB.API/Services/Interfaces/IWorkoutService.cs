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

        Guid[]? AddExercisesToWorkout(Workout workout, List<Guid> exercisesIds, User owner);

        Guid[]? UpdateWorkout(Workout workout, WorkoutCreateUpdateModel update, User owner);

        void RemoveAllWorkoutExercises(Workout workout);
    }
}
