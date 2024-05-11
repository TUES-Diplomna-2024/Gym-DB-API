using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IWorkoutExerciseRepository
    {
        Task<List<WorkoutExercise>> GetAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId);

        Task<bool> AreAllExercisesInWorkoutAsync(Guid workoutId, List<Guid> exercisesIds);

        Task AddWorkoutExerciseAsync(WorkoutExercise workoutExercise);

        Task UpdateWorkoutExercisePossitionAsync(WorkoutExercise workoutExercise, uint possition);

        Task RemoveWorkoutExerciseAsync(WorkoutExercise workoutExercise);

        Task RemoveAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId);
    }
}
