using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IExerciseRepository
    {
        Task<Exercise?> GetExerciseByIdAsync(Guid id);

        Task<List<Exercise>> GetAllUserCustomExercisesAsync(Guid userId);

        Task<List<Exercise>> FindAllExercisesMatchingNameAsync(string name);

        Task AddExerciseAsync(Exercise exercise);

        Task UpdateExerciseAsync(Exercise exercise);

        Task RemoveExerciseAsync(Exercise exercise);
    }
}
