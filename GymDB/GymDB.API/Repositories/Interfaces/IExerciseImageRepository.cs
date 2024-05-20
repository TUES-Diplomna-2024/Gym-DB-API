using GymDB.API.Data.Entities;

namespace GymDB.API.Repositories.Interfaces
{
    public interface IExerciseImageRepository
    {
        Task<List<ExerciseImage>> GetAllExerciseImagesByExerciseIdAsync(Guid exerciseId);

        Task AddExerciseImageAsync(ExerciseImage exerciseImage);

        Task UpdateExerciseImagePossitionAsync(ExerciseImage exerciseImage, uint possition);

        Task RemoveExerciseImageAsync(ExerciseImage exerciseImage);

        Task RemoveExerciseImageRangeAsync(List<ExerciseImage> exerciseImages);
    }
}
