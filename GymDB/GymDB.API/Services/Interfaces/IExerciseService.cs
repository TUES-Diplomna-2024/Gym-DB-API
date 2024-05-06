using GymDB.API.Models.Exercise;
using GymDB.API.Data.Enums;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseService
    {
        Task CreateNewExerciseAsync(HttpContext context, ExerciseCreateModel createModel);

        Task<ExerciseViewModel> GetExerciseViewByIdAsync(HttpContext context, Guid exerciseId);

        Task<List<ExercisePreviewModel>> GetCurrUserCustomExercisesPreviewsAsync(HttpContext context);

        Task<List<ExercisePreviewModel>> SearchExercisesPreviewsAsync(HttpContext context, string name);

        Task<List<ExercisePreviewModel>> SearchExercisesPreviewsAsync(ExerciseSearchModel searchModel);

        Task UpdateExerciseByIdAsync(HttpContext context, Guid exerciseId, ExerciseUpdateModel updateModel);

        Task UpdateExerciseVisibilityAsync(Guid exerciseId, ExerciseVisibility visibility);

        Task RemoveExerciseByIdAsync(HttpContext context, Guid exerciseId);
    }
}
