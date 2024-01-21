using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IWorkoutService
    {
        void AddWorkout(Workout workout);

        void AddExercisesToWorkout(Workout workout, List<Exercise> exercises);
    }
}
