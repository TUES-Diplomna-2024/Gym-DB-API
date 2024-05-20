using GymDB.API.Models.Exercise;
using GymDB.API.Data.Enums;
using GymDB.API.Data.Entities;

namespace GymDB.API.Services.Interfaces
{
    public interface IExerciseService
    {
        Task CreateNewExerciseAsync(HttpContext context, ExerciseCreateModel createModel);

        Task<Exercise> GetExerciseByIdAsync(User user, Guid exerciseId, ExerciseValidation validation = ExerciseValidation.Access);

        Task<ExerciseViewModel> GetExerciseViewByIdAsync(HttpContext context, Guid exerciseId);

        Task<List<ExercisePreviewModel>> GetCurrUserCustomExercisesPreviewsAsync(HttpContext context);

        Task<List<ExercisePreviewModel>> FindPublicAndCustomExercisesPreviewsAsync(HttpContext context, string name);

        Task<List<ExercisePreviewModel>> FindAdminCreatedExercisesPreviewsAsync(ExerciseSearchModel searchModel);

        Task UpdateExerciseByIdAsync(HttpContext context, Guid exerciseId, ExerciseUpdateModel updateModel);

        Task UpdateExerciseVisibilityAsync(Guid exerciseId, ExerciseVisibility visibility);

        Task RemoveExerciseByIdAsync(HttpContext context, Guid exerciseId);

        Task RemoveAllUserCustomExercisesAsync(Guid userId);
    }
}
