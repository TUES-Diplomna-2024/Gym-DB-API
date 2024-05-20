using GymDB.API.Data.Entities;
using GymDB.API.Models.ExerciseImage;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseImageService
    {
        Task<List<ExerciseImageViewModel>> GetExerciseImagesViewsAsync(Guid exerciseId);

        Task AddImagesToExerciseAsync(Exercise exercise, List<IFormFile> images);

        Task RemoveExerciseImagesAsync(Exercise exercise, List<Guid> exerciseImagesIds);

        Task RemoveAllExerciseImagesAsync(Guid exerciseId);
    }
}
