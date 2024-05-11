using GymDB.API.Data.Entities;
using GymDB.API.Models.Exercise;

namespace GymDB.API.Services.Interfaces
{
    public interface IWorkoutExerciseService
    {
        Task<List<ExercisePreviewModel>> GetWorkoutExercisesPreviewsAsync(Guid workoutId);

        Task UpdateWorkoutExercisesAsync(Workout workout, List<Guid>? exercisesIds);
    }
}
