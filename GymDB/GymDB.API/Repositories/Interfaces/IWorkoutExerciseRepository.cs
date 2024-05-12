using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IWorkoutExerciseRepository
    {
        Task<List<WorkoutExercise>> GetAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId);

        Task<List<WorkoutExercise>> GetAllWorkoutExercisesByExerciseIdAsync(Guid exerciseId);

        Task<bool> AreAllExercisesInWorkoutAsync(Guid workoutId, List<Guid> exercisesIds);

        Task AddWorkoutExerciseAsync(WorkoutExercise workoutExercise);

        Task UpdateWorkoutExerciseRangeAsync(List<WorkoutExercise> workoutExercises);

        Task RemoveWorkoutExerciseRangeAsync(List<WorkoutExercise> workoutExercises);

        Task RemoveAllWorkoutExercisesByWorkoutIdAsync(Guid workoutId);
    }
}
