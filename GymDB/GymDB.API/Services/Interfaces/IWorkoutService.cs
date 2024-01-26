using GymDB.API.Data.Entities;
using GymDB.API.Models.Workout;

namespace GymDB.API.Services.Interfaces
{
    public interface IWorkoutService
    {
        List<Workout> GetUserWorkouts(User user);

        List<WorkoutPreviewModel> GetWorkoutsPreviews(List<Workout> workouts);

        List<Workout> GetWorkoutsContainingExercise(Exercise exercise);

        List<Exercise> GetWorkoutExercises(Workout workout);

        WorkoutWithExercisesModel GetWorkoutWithExercises(Workout workout);

        Workout? GetWorkoutById(Guid id);

        void AddWorkout(Workout workout);

        void AddExercisesToWorkout(Workout workout, List<Exercise> exercises);

        Guid[]? AddExercisesToWorkout(Workout workout, List<Guid> exercisesIds, User owner);

        Guid[]? UpdateWorkout(Workout workout, WorkoutCreateUpdateModel update, User owner);

        void RemoveAllWorkoutExercises(Workout workout);

        void RemoveExerciseFromAllWorkouts(Exercise exercise, bool excludeOwnerWorkouts);

        void RemoveWorkout(Workout workout);

        void RemoveAllUserWorkouts(User user);
    }
}
